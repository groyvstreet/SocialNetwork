﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.UserDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using System.Text.Json;

namespace PostService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly IPostRepository _postRepository;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IMapper mapper,
                           ICommentRepository commentRepository,
                           ICommentLikeRepository commentLikeRepository,
                           IPostRepository postRepository,
                           IPostLikeRepository postLikeRepository,
                           ILogger<UserService> logger)
        {
            _mapper = mapper;
            _commentRepository = commentRepository;
            _commentLikeRepository = commentLikeRepository;
            _postRepository = postRepository;
            _postLikeRepository = postLikeRepository;
            _logger = logger;
        }

        public async Task<List<GetUserDTO>> GetUsersLikedByCommentIdAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {commentId}");
            }

            var commentLikes = await _commentLikeRepository.GetCommentLikesWithUserByCommentIdAsync(commentId);
            var users = commentLikes.Select(commentLike => commentLike.User);
            var getUserDTOs = users.Select(_mapper.Map<GetUserDTO>).ToList();

            _logger.LogInformation("users - {users} getted", JsonSerializer.Serialize(users));

            return getUserDTOs;
        }

        public async Task<List<GetUserDTO>> GetUsersLikedByPostIdAsync(Guid postId)
        {
            var post = await _postRepository.GetFirstOrDefaultByAsync(post => post.Id == postId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {postId}");
            }

            var postLikes = await _postLikeRepository.GetPostLikesWithUserByPostIdAsync(postId);
            var users = postLikes.Select(postLike => postLike.User);
            var getUserDTOs = users.Select(_mapper.Map<GetUserDTO>).ToList();

            _logger.LogInformation("users - {users} getted", JsonSerializer.Serialize(users));

            return getUserDTOs;
        }
    }
}
