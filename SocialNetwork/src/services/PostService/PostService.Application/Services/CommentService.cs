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
        private readonly IMapper mapper;
        private readonly ICommentRepository commentRepository;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly ICommentLikeRepository commentLikeRepository;

        public CommentService(IMapper mapper,
                           ICommentRepository commentRepository,
                           IPostRepository postRepository,
                           IUserRepository userRepository,
                           ICommentLikeRepository commentLikeRepository)
        {
            this.mapper = mapper;
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.commentLikeRepository = commentLikeRepository;
        }

        public async Task<List<GetCommentDTO>> GetCommentsAsync()
        {
            var comments = await commentRepository.GetCommentsAsync();
            var getCommentDTOs = comments.Select(mapper.Map<GetCommentDTO>).ToList();

            return getCommentDTOs;
        }

        public async Task<GetCommentDTO> GetCommentByIdAsync(Guid id)
        {
            var comment = await commentRepository.GetCommentByIdAsync(id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {id}");
            }

            var getCommentDTO = mapper.Map<GetCommentDTO>(comment);

            return getCommentDTO;
        }

        public async Task<List<GetCommentDTO>> GetCommentsByPostIdAsync(Guid postId)
        {
            var post = await postRepository.GetPostByIdAsync(postId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {postId}");
            }

            var comments = await commentRepository.GetCommentsByPostIdAsync(postId);
            var getCommentDTOs = comments.Select(mapper.Map<GetCommentDTO>).ToList();

            return getCommentDTOs;
        }

        public async Task<List<GetCommentDTO>> GetLikedCommentsByUserIdAsync(Guid userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var commentLikes = await commentLikeRepository.GetCommentLikesByUserIdAsync(userId);
            var comments = commentLikes.Select(cl => commentRepository.GetCommentByIdAsync(cl.CommentId).Result);
            var getCommentDTOs = comments.Select(mapper.Map<GetCommentDTO>).ToList();

            return getCommentDTOs;
        }

        public async Task<GetCommentDTO> AddCommentAsync(AddCommentDTO addCommentDTO)
        {
            var post = await postRepository.GetPostByIdAsync(addCommentDTO.PostId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {addCommentDTO.PostId}");
            }

            var user = await userRepository.GetUserByIdAsync(addCommentDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addCommentDTO.UserId}");
            }

            var comment = mapper.Map<Comment>(addCommentDTO);
            comment.DateTime = DateTimeOffset.Now;
            await commentRepository.AddCommentAsync(comment);
            var getCommentDTO = mapper.Map<GetCommentDTO>(comment);

            post.CommentCount++;
            await postRepository.UpdatePostAsync(post);

            return getCommentDTO;
        }

        public async Task<GetCommentDTO> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO)
        {
            var comment = await commentRepository.GetCommentByIdAsync(updateCommentDTO.Id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {updateCommentDTO.Id}");
            }

            comment.Text = updateCommentDTO.Text;
            await commentRepository.UpdateCommentAsync(comment);
            var getCommentDTO = mapper.Map<GetCommentDTO>(comment);

            return getCommentDTO;
        }

        public async Task RemoveCommentByIdAsync(Guid id)
        {
            var comment = await commentRepository.GetCommentByIdAsync(id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {id}");
            }

            await commentRepository.RemoveCommentAsync(comment);

            var post = await postRepository.GetPostByIdAsync(comment.PostId);
            post!.CommentCount--;
            await postRepository.UpdatePostAsync(post);
        }
    }
}
