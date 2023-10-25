using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using System.Text.Json;

namespace PostService.Application.Services
{
    public class PostLikeService : IPostLikeService
    {
        private readonly IMapper _mapper;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PostLikeService> _logger;
        private readonly ICacheRepository<Post> _postCacheRepository;
        private readonly ICacheRepository<User> _userCacheRepository;
        private readonly ICacheRepository<PostLike> _postLikeCacheRepository;

        public PostLikeService(IMapper mapper,
                               IPostLikeRepository postLikeRepository,
                               IPostRepository postRepository,
                               IUserRepository userRepository,
                               ILogger<PostLikeService> logger)
                               ICacheRepository<Post> postCacheRepository,
                               ICacheRepository<User> userCacheRepository,
                               ICacheRepository<PostLike> postLikeCacheRepository)
        {
            _mapper = mapper;
            _postLikeRepository = postLikeRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _logger = logger;
            _userCacheRepository = userCacheRepository;
            _postCacheRepository = postCacheRepository;
            _postLikeCacheRepository = postLikeCacheRepository;
        }

        public async Task<GetPostLikeDTO> AddPostLikeAsync(AddRemovePostLikeDTO addPostLikeDTO, Guid authenticatedUserId)
        {
            if (addPostLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var post = await _postCacheRepository.GetAsync(addPostLikeDTO.PostId.ToString());

            if (post is null)
            {
                post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == addPostLikeDTO.PostId);

                if (post is null)
                {
                    throw new NotFoundException($"no such post with id = {addPostLikeDTO.PostId}");
                }
            }
            else
            {
                _postRepository.Update(post);
            }

            var user = await _userCacheRepository.GetAsync(addPostLikeDTO.UserId.ToString());

            if (user is null)
            {
                user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == addPostLikeDTO.UserId);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {addPostLikeDTO.UserId}");
                }

                await _userCacheRepository.SetAsync(user.Id.ToString(), user);
            }

            var postLike = await _postLikeCacheRepository.GetAsync($"{addPostLikeDTO.PostId}&{addPostLikeDTO.UserId}");

            if (postLike is null)
            {
                postLike = await _postLikeRepository.GetFirstOrDefaultByAsync(pl =>
                    pl.PostId == addPostLikeDTO.PostId && pl.UserId == addPostLikeDTO.UserId);

                if (postLike is not null)
                {
                    await _postLikeCacheRepository.SetAsync($"{addPostLikeDTO.PostId}&{addPostLikeDTO.UserId}", postLike);

                    throw new AlreadyExistsException($"post like with postId = {addPostLikeDTO.PostId} and userId = {addPostLikeDTO.UserId} already exists");
                }
            }
            else
            {
                throw new AlreadyExistsException($"post like with postId = {addPostLikeDTO.PostId} and userId = {addPostLikeDTO.UserId} already exists");
            }

            postLike = _mapper.Map<PostLike>(addPostLikeDTO);
            await _postLikeRepository.AddAsync(postLike);
            await _postLikeRepository.SaveChangesAsync();
            var getPostLikeDTO = _mapper.Map<GetPostLikeDTO>(postLike);

            post.LikeCount++;
            await _postRepository.SaveChangesAsync();

            await _postCacheRepository.SetAsync(post.Id.ToString(), post);
            
            _logger.LogInformation("postLike - {postLike} added", JsonSerializer.Serialize(postLike));

            return getPostLikeDTO;
        }

        public async Task RemovePostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO, Guid authenticatedUserId)
        {
            if (addRemovePostLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var postLike = await _postLikeCacheRepository.GetAsync($"{addRemovePostLikeDTO.PostId}&{addRemovePostLikeDTO.UserId}");

            if (postLike is null)
            {
                postLike = await _postLikeRepository.GetFirstOrDefaultByAsync(pl =>
                    pl.PostId == addRemovePostLikeDTO.PostId && pl.UserId == addRemovePostLikeDTO.UserId);

                if (postLike is null)
                {
                    throw new NotFoundException($"no such post like with postId = {addRemovePostLikeDTO.PostId} and userId = {addRemovePostLikeDTO.UserId}");
                }
            }

            _postLikeRepository.Remove(postLike);
            await _postLikeRepository.SaveChangesAsync();

            await _postLikeCacheRepository.RemoveAsync($"{addRemovePostLikeDTO.PostId}&{addRemovePostLikeDTO.UserId}");

            var post = await _postCacheRepository.GetAsync(postLike.PostId.ToString());

            if (post is null)
            {
                post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == postLike.PostId);
            }
            else
            {
                _postRepository.Update(post);
            }
            
            post!.LikeCount--;
            await _postRepository.SaveChangesAsync();

            await _postCacheRepository.SetAsync(post.Id.ToString(), post);
            
            _logger.LogInformation("postLike - {postLike} removed", JsonSerializer.Serialize(postLike));
        }
    }
}
