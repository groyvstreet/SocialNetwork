using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using System.Text.Json;

namespace PostService.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly ILogger<PostService> _logger;

        public PostService(IMapper mapper,
                           IPostRepository postRepository,
                           IUserRepository userRepository,
                           IPostLikeRepository postLikeRepository,
                           ILogger<PostService> logger)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _postLikeRepository = postLikeRepository;
            _logger = logger;
        }

        public async Task<List<GetPostDTO>> GetPostsAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            var getPostDTOs = posts.Select(_mapper.Map<GetPostDTO>).ToList();

            _logger.LogInformation("posts - {posts} getted", JsonSerializer.Serialize(posts));

            return getPostDTOs;
        }

        public async Task<GetPostDTO> GetPostByIdAsync(Guid id)
        {
            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == id);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {id}");
            }

            var getPostDTO = _mapper.Map<GetPostDTO>(post);

            _logger.LogInformation("post - {post} getted", JsonSerializer.Serialize(post));

            return getPostDTO;
        }

        public async Task<List<GetPostDTO>> GetPostsByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserWithPostsByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var getPostDTOs = user.Posts.Select(_mapper.Map<GetPostDTO>).ToList();

            _logger.LogInformation("posts - {posts} getted", JsonSerializer.Serialize(user.Posts));

            return getPostDTOs;
        }

        public async Task<List<GetPostDTO>> GetLikedPostsByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var postLikes = await _postLikeRepository.GetPostLikesWithPostByUserIdAsync(userId);
            var posts = postLikes.Select(postLike => postLike.Post);
            var getPostDTOs = posts.Select(_mapper.Map<GetPostDTO>).ToList();

            _logger.LogInformation("posts - {posts} getted", JsonSerializer.Serialize(posts));

            return getPostDTOs;
        }

        public async Task<GetPostDTO> AddPostAsync(AddPostDTO addPostDTO, Guid authenticatedUserId)
        {
            if (addPostDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == addPostDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addPostDTO.UserId}");
            }

            var post = _mapper.Map<Post>(addPostDTO);
            post.DateTime = DateTimeOffset.Now;
            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();
            var getPostDTO = _mapper.Map<GetPostDTO>(post);

            _logger.LogInformation("post - {post} added", JsonSerializer.Serialize(post));

            return getPostDTO;
        }

        public async Task<GetPostDTO> UpdatePostAsync(UpdatePostDTO updatePostDTO, Guid authenticatedUserId)
        {
            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == updatePostDTO.Id);
            
            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {updatePostDTO.Id}");
            }

            if (post.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            post.Text = updatePostDTO.Text;
            await _postRepository.SaveChangesAsync();
            var getPostDTO = _mapper.Map<GetPostDTO>(post);

            _logger.LogInformation("post - {post} updated", JsonSerializer.Serialize(post));

            return getPostDTO;
        }

        public async Task RemovePostByIdAsync(Guid id, Guid authenticatedUserId)
        {
            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == id);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {id}");
            }

            if (post.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            _postRepository.Remove(post);
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("post - {post} removed", JsonSerializer.Serialize(post));
        }
    }
}
