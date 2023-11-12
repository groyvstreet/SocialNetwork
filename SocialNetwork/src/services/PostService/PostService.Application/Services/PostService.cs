using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces;
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
        private readonly ICacheRepository<Post> _postCacheRepository;
        private readonly ICacheRepository<User> _userCacheRepository;

        public PostService(IMapper mapper,
                           IPostRepository postRepository,
                           IUserRepository userRepository,
                           IPostLikeRepository postLikeRepository,
                           ILogger<PostService> logger,
                           ICacheRepository<Post> postCacheRepository,
                           ICacheRepository<User> userCacheRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _postLikeRepository = postLikeRepository;
            _logger = logger;
            _postCacheRepository = postCacheRepository;
            _userCacheRepository = userCacheRepository;
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
            var post = await _postCacheRepository.GetAsync(id.ToString());

            if (post is null)
            {
                post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == id);

                if (post is null)
                {
                    throw new NotFoundException($"no such post with id = {id}");
                }

                await _postCacheRepository.SetAsync(post.Id.ToString(), post);
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

            var user = await _userCacheRepository.GetAsync(addPostDTO.UserId.ToString());

            if (user is null)
            {
                user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == addPostDTO.UserId);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {addPostDTO.UserId}");
                }

                await _userCacheRepository.SetAsync(user.Id.ToString(), user);
            }

            var post = _mapper.Map<Post>(addPostDTO);
            post.DateTime = DateTimeOffset.UtcNow;
            await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();
            var getPostDTO = _mapper.Map<GetPostDTO>(post);

            await _postCacheRepository.SetAsync(post.Id.ToString(), post);
            
            _logger.LogInformation("post - {post} added", JsonSerializer.Serialize(post));
            
            return getPostDTO;
        }

        public async Task<GetPostDTO> UpdatePostAsync(UpdatePostDTO updatePostDTO, Guid authenticatedUserId)
        {
            var post = await _postCacheRepository.GetAsync(updatePostDTO.Id.ToString());

            if (post is null)
            {
                post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == updatePostDTO.Id);

                if (post is null)
                {
                    throw new NotFoundException($"no such post with id = {updatePostDTO.Id}");
                }

                await _postCacheRepository.SetAsync(post.Id.ToString(), post);
            }
            else
            {
                _postRepository.Update(post);
            }

            if (post.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            post.Text = updatePostDTO.Text;
            await _postRepository.SaveChangesAsync();
            var getPostDTO = _mapper.Map<GetPostDTO>(post);

            await _postCacheRepository.SetAsync(post.Id.ToString(), post);
            
            _logger.LogInformation("post - {post} updated", JsonSerializer.Serialize(post));

            return getPostDTO;
        }

        public async Task RemovePostByIdAsync(Guid id, Guid authenticatedUserId)
        {
            var post = await _postCacheRepository.GetAsync(id.ToString());

            if (post is null)
            {
                post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == id);

                if (post is null)
                {
                    throw new NotFoundException($"no such post with id = {id}");
                }

                await _postCacheRepository.SetAsync(post.Id.ToString(), post);
            }

            if (post.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            _postRepository.Remove(post);
            await _postRepository.SaveChangesAsync();

            await _postCacheRepository.RemoveAsync(id.ToString());
            
            _logger.LogInformation("post - {post} removed", JsonSerializer.Serialize(post));
        }
    }
}
