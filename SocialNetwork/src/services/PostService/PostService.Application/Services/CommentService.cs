using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using System.Text.Json;

namespace PostService.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly ILogger<CommentService> _logger;

        public CommentService(IMapper mapper,
                              ICommentRepository commentRepository,
                              IPostRepository postRepository,
                              IUserRepository userRepository,
                              ICommentLikeRepository commentLikeRepository,
                              ILogger<CommentService> logger)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentLikeRepository = commentLikeRepository;
            _logger = logger;
        }

        public async Task<List<GetCommentDTO>> GetCommentsAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            var getCommentDTOs = comments.Select(_mapper.Map<GetCommentDTO>).ToList();

            _logger.LogInformation("comments - {comments} getted", JsonSerializer.Serialize(comments));

            return getCommentDTOs;
        }

        public async Task<GetCommentDTO> GetCommentByIdAsync(Guid id)
        {
            var comment = await _commentRepository.GetFirstOrDefaultByAsync(chat => chat.Id == id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {id}");
            }

            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            _logger.LogInformation("comment - {comment} getted", JsonSerializer.Serialize(comment));

            return getCommentDTO;
        }

        public async Task<List<GetCommentDTO>> GetCommentsByPostIdAsync(Guid postId)
        {
            var post = await _postRepository.GetPostWithCommentsByIdAsync(postId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {postId}");
            }

            var getCommentDTOs = post.Comments.Select(_mapper.Map<GetCommentDTO>).ToList();

            _logger.LogInformation("comments - {comments} getted", JsonSerializer.Serialize(post.Comments));

            return getCommentDTOs;
        }

        public async Task<List<GetCommentDTO>> GetLikedCommentsByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var commentLikes = await _commentLikeRepository.GetCommentLikesWithCommentByUserIdAsync(userId);
            var comments = commentLikes.Select(commentLike => commentLike.Comment);
            var getCommentDTOs = comments.Select(_mapper.Map<GetCommentDTO>).ToList();

            _logger.LogInformation("comments - {comments} getted", JsonSerializer.Serialize(comments));

            return getCommentDTOs;
        }

        public async Task<GetCommentDTO> AddCommentAsync(AddCommentDTO addCommentDTO, Guid authenticatedUserId)
        {
            if (addCommentDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == addCommentDTO.PostId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {addCommentDTO.PostId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == addCommentDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addCommentDTO.UserId}");
            }

            var comment = _mapper.Map<Comment>(addCommentDTO);
            comment.DateTime = DateTimeOffset.Now;
            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();
            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            post.CommentCount++;
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("comment - {comment} added", JsonSerializer.Serialize(comment));

            return getCommentDTO;
        }

        public async Task<GetCommentDTO> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO, Guid authenticatedUserId)
        {
            var comment = await _commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == updateCommentDTO.Id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {updateCommentDTO.Id}");
            }

            if (comment.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            comment.Text = updateCommentDTO.Text;
            await _commentRepository.SaveChangesAsync();
            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            _logger.LogInformation("comment - {comment} updated", JsonSerializer.Serialize(comment));

            return getCommentDTO;
        }

        public async Task RemoveCommentByIdAsync(Guid id, Guid authenticatedUserId)
        {
            var comment = await _commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == id);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {id}");
            }

            if (comment.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            _commentRepository.Remove(comment);
            await _commentRepository.SaveChangesAsync();

            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == comment.PostId);
            post!.CommentCount--;
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("comment - {comment} removed", JsonSerializer.Serialize(comment));
        }
    }
}
