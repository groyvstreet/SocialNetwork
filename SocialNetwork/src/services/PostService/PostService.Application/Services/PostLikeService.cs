using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Exceptions;
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

        public PostLikeService(IMapper mapper,
                               IPostLikeRepository postLikeRepository,
                               IPostRepository postRepository,
                               IUserRepository userRepository,
                               ILogger<PostLikeService> logger)
        {
            _mapper = mapper;
            _postLikeRepository = postLikeRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<GetPostLikeDTO> AddPostLikeAsync(AddRemovePostLikeDTO addPostLikeDTO, Guid authenticatedUserId)
        {
            if (addPostLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == addPostLikeDTO.PostId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {addPostLikeDTO.PostId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == addPostLikeDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addPostLikeDTO.UserId}");
            }

            var postLike = await _postLikeRepository.GetFirstOrDefaultByAsync(postLike => 
                postLike.PostId == addPostLikeDTO.PostId && postLike.UserId == addPostLikeDTO.UserId);

            if (postLike is not null)
            {
                throw new AlreadyExistsException($"post like with postId = {addPostLikeDTO.PostId} and userId = {addPostLikeDTO.UserId} already exists");
            }

            postLike = _mapper.Map<PostLike>(addPostLikeDTO);
            await _postLikeRepository.AddAsync(postLike);
            await _postLikeRepository.SaveChangesAsync();
            var getPostLikeDTO = _mapper.Map<GetPostLikeDTO>(postLike);

            post.LikeCount++;
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("postLike - {postLike} added", JsonSerializer.Serialize(postLike));

            return getPostLikeDTO;
        }

        public async Task RemovePostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO, Guid authenticatedUserId)
        {
            if (addRemovePostLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var postLike = await _postLikeRepository.GetFirstOrDefaultByAsync(postLike => 
                postLike.PostId == addRemovePostLikeDTO.PostId && postLike.UserId == addRemovePostLikeDTO.UserId);

            if (postLike is null)
            {
                throw new NotFoundException($"no such post like with postId = {addRemovePostLikeDTO.PostId} and userId = {addRemovePostLikeDTO.UserId}");
            }

            _postLikeRepository.Remove(postLike);
            await _postLikeRepository.SaveChangesAsync();

            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == postLike.PostId);
            post!.LikeCount--;
            await _postRepository.SaveChangesAsync();

            _logger.LogInformation("postLike - {postLike} removed", JsonSerializer.Serialize(postLike));
        }
    }
}
