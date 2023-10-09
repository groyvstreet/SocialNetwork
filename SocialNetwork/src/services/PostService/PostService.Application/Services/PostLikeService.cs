﻿using AutoMapper;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class PostLikeService : IPostLikeService
    {
        private readonly IMapper _mapper;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public PostLikeService(IMapper mapper,
                               IPostLikeRepository postLikeRepository,
                               IPostRepository postRepository,
                               IUserRepository userRepository)
        {
            _mapper = mapper;
            _postLikeRepository = postLikeRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<GetPostLikeDTO> AddPostLikeAsync(AddRemovePostLikeDTO addPostLikeDTO, Guid authenticatedUserId)
        {
            if (addPostLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == addPostLikeDTO.PostId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {addPostLikeDTO.PostId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == addPostLikeDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addPostLikeDTO.UserId}");
            }

            var postLike = await _postLikeRepository.GetFirstOrDefaultByAsync(pl => 
                pl.PostId == addPostLikeDTO.PostId && pl.UserId == addPostLikeDTO.UserId);

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

            return getPostLikeDTO;
        }

        public async Task RemovePostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO, Guid authenticatedUserId)
        {
            if (addRemovePostLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var postLike = await _postLikeRepository.GetFirstOrDefaultByAsync(pl => 
                pl.PostId == addRemovePostLikeDTO.PostId && pl.UserId == addRemovePostLikeDTO.UserId);

            if (postLike is null)
            {
                throw new NotFoundException($"no such post like with postId = {addRemovePostLikeDTO.PostId} and userId = {addRemovePostLikeDTO.UserId}");
            }

            _postLikeRepository.Remove(postLike);
            await _postLikeRepository.SaveChangesAsync();

            var post = await _postRepository.GetFirstOrDefaultByAsync(p => p.Id == postLike.PostId);
            post!.LikeCount--;
            await _postRepository.SaveChangesAsync();
        }
    }
}
