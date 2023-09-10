using AutoMapper;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommentLikeRepository _commentLikeRepository;

        public CommentService(IMapper mapper,
                           ICommentRepository commentRepository,
                           IPostRepository postRepository,
                           IUserRepository userRepository,
                           ICommentLikeRepository commentLikeRepository)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentLikeRepository = commentLikeRepository;
        }

        public async Task<List<GetCommentDTO>> GetCommentsAsync()
        {
            var comments = await _commentRepository.GetCommentsAsync();
            var getCommentDTOs = comments.Select(_mapper.Map<GetCommentDTO>).ToList();

            return getCommentDTOs;
        }

        public async Task<GetCommentDTO> GetCommentByIdAsync(Guid id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {id}");
            }

            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            return getCommentDTO;
        }

        public async Task<List<GetCommentDTO>> GetCommentsByPostIdAsync(Guid postId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {postId}");
            }

            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId);
            var getCommentDTOs = comments.Select(_mapper.Map<GetCommentDTO>).ToList();

            return getCommentDTOs;
        }

        public async Task<List<GetCommentDTO>> GetLikedCommentsByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var commentLikes = await _commentLikeRepository.GetCommentLikesByUserIdAsync(userId);
            var comments = commentLikes.Select(cl => _commentRepository.GetCommentByIdAsync(cl.CommentId).Result);
            var getCommentDTOs = comments.Select(_mapper.Map<GetCommentDTO>).ToList();

            return getCommentDTOs;
        }

        public async Task<GetCommentDTO> AddCommentAsync(AddCommentDTO addCommentDTO)
        {
            var post = await _postRepository.GetPostByIdAsync(addCommentDTO.PostId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {addCommentDTO.PostId}");
            }

            var user = await _userRepository.GetUserByIdAsync(addCommentDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addCommentDTO.UserId}");
            }

            var comment = _mapper.Map<Comment>(addCommentDTO);
            comment.DateTime = DateTimeOffset.Now;
            await _commentRepository.AddCommentAsync(comment);
            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            post.CommentCount++;
            await _postRepository.UpdatePostAsync(post);

            return getCommentDTO;
        }

        public async Task<GetCommentDTO> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(updateCommentDTO.Id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {updateCommentDTO.Id}");
            }

            comment.Text = updateCommentDTO.Text;
            await _commentRepository.UpdateCommentAsync(comment);
            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            return getCommentDTO;
        }

        public async Task RemoveCommentByIdAsync(Guid id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {id}");
            }

            await _commentRepository.RemoveCommentAsync(comment);

            var post = await _postRepository.GetPostByIdAsync(comment.PostId);
            post!.CommentCount--;
            await _postRepository.UpdatePostAsync(post);
        }
    }
}
