using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces;
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
        private readonly ICacheRepository<Comment> _commentCacheRepository;
        private readonly ICacheRepository<Post> _postCacheRepository;
        private readonly ICacheRepository<User> _userCacheRepository;

        public CommentService(IMapper mapper,
                              ICommentRepository commentRepository,
                              IPostRepository postRepository,
                              IUserRepository userRepository,
                              ICommentLikeRepository commentLikeRepository,
                              ILogger<CommentService> logger,
                              ICacheRepository<Comment> commentCacheRepository,
                              ICacheRepository<Post> postCacheRepository,
                              ICacheRepository<User> userCacheRepository)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentLikeRepository = commentLikeRepository;
            _logger = logger;
            _commentCacheRepository = commentCacheRepository;
            _postCacheRepository = postCacheRepository;
            _userCacheRepository = userCacheRepository;
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
            var comment = await _commentCacheRepository.GetAsync(id.ToString());

            if (comment is null)
            {
                comment = await _commentRepository.GetFirstOrDefaultByAsync(c => c.Id == id);

                if (comment is null)
                {
                    throw new NotFoundException($"no such comment with id = {id}");
                }

                await _commentCacheRepository.SetAsync(comment.Id.ToString(), comment);
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
            var user = await _userCacheRepository.GetAsync(userId.ToString());

            if (user is null)
            {
                user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == userId);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {userId}");
                }

                await _userCacheRepository.SetAsync(user.Id.ToString(), user);
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

            var post = await _postCacheRepository.GetAsync(addCommentDTO.PostId.ToString());

            if (post is null)
            {
                post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == addCommentDTO.PostId);

                if (post is null)
                {
                    throw new NotFoundException($"no such post with id = {addCommentDTO.PostId}");
                }
            }
            else
            {
                _postRepository.Update(post);
            }

            var user = await _userCacheRepository.GetAsync(addCommentDTO.UserId.ToString());

            if (user is null)
            {
                user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == addCommentDTO.UserId);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {addCommentDTO.UserId}");
                }

                await _userCacheRepository.SetAsync(user.Id.ToString(), user);
            }

            var comment = _mapper.Map<Comment>(addCommentDTO);
            comment.DateTime = DateTimeOffset.UtcNow;
            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();
            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            await _commentCacheRepository.SetAsync(comment.Id.ToString(), comment);

            post.CommentCount++;
            await _postRepository.SaveChangesAsync();

            await _postCacheRepository.SetAsync(post.Id.ToString(), post);
            
             _logger.LogInformation("comment - {comment} added", JsonSerializer.Serialize(comment));

            return getCommentDTO;
        }

        public async Task<GetCommentDTO> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO, Guid authenticatedUserId)
        {
            var comment = await _commentCacheRepository.GetAsync(updateCommentDTO.Id.ToString());

            if (comment is null)
            {
                comment = await _commentRepository.GetFirstOrDefaultByAsync(c => c.Id == updateCommentDTO.Id);

                if (comment is null)
                {
                    throw new NotFoundException($"no such comment with id = {updateCommentDTO.Id}");
                }
            }
            else
            {
                _commentRepository.Update(comment);
            }

            if (comment.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            comment.Text = updateCommentDTO.Text;
            await _commentRepository.SaveChangesAsync();
            var getCommentDTO = _mapper.Map<GetCommentDTO>(comment);

            await _commentCacheRepository.SetAsync(comment.Id.ToString(), comment);
            
            _logger.LogInformation("comment - {comment} updated", JsonSerializer.Serialize(comment));

            return getCommentDTO;
        }

        public async Task RemoveCommentByIdAsync(Guid id, Guid authenticatedUserId)
        {
            var comment = await _commentCacheRepository.GetAsync(id.ToString());

            if (comment is null)
            {
                comment = await _commentRepository.GetFirstOrDefaultByAsync(c => c.Id == id);

                if (comment is null)
                {
                    throw new NotFoundException($"no such comment with id = {id}");
                }
            }

            if (comment.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            _commentRepository.Remove(comment);
            await _commentRepository.SaveChangesAsync();

            var post = await _postCacheRepository.GetAsync(comment.PostId.ToString());

            if (post is null)
            {
                post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == comment.PostId);
            }
            else
            {
                _postRepository.Update(post);
            }

            post!.CommentCount--;
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("comment - {comment} removed", JsonSerializer.Serialize(comment));
        }
    }
}
