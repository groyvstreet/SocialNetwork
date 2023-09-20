﻿using AutoMapper;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly IMapper _mapper;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;

        public CommentLikeService(IMapper mapper,
                                  ICommentLikeRepository commentLikeRepository,
                                  ICommentRepository commentRepository,
                                  IUserRepository userRepository)
        {
            _mapper = mapper;
            _commentLikeRepository = commentLikeRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        public async Task<GetCommentLikeDTO> AddCommentLikeAsync(AddRemoveCommentLikeDTO addCommentLikeDTO)
        {
            var comment = await _commentRepository.GetFirstOrDefaultByAsync(c => c.Id == addCommentLikeDTO.CommentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {addCommentLikeDTO.CommentId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == addCommentLikeDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addCommentLikeDTO.UserId}");
            }

            var commentLike = await _commentLikeRepository.GetFirstOrDefaultByAsync(cl => 
                cl.CommentId == addCommentLikeDTO.CommentId && cl.UserId == addCommentLikeDTO.UserId);

            if (commentLike is not null)
            {
                throw new AlreadyExistsException($"comment like with commentId = {addCommentLikeDTO.CommentId} and userId = {addCommentLikeDTO.UserId} already exists");
            }

            commentLike = _mapper.Map<CommentLike>(addCommentLikeDTO);
            await _commentLikeRepository.AddAsync(commentLike);
            await _commentLikeRepository.SaveChangesAsync();
            var getCommentsUserDTO = _mapper.Map<GetCommentLikeDTO>(commentLike);

            comment.LikeCount++;
            await _commentRepository.SaveChangesAsync();

            return getCommentsUserDTO;
        }

        public async Task RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            var commentLike = await _commentLikeRepository.GetFirstOrDefaultByAsync(cl => 
                cl.CommentId == addRemoveCommentLikeDTO.CommentId && cl.UserId == addRemoveCommentLikeDTO.UserId);

            if (commentLike is null)
            {
                throw new NotFoundException($"no such comment like with commentId = {addRemoveCommentLikeDTO.CommentId} and userId = {addRemoveCommentLikeDTO.UserId}");
            }

            _commentLikeRepository.Remove(commentLike);
            await _commentLikeRepository.SaveChangesAsync();

            var comment = await _commentRepository.GetFirstOrDefaultByAsync(c => c.Id == commentLike.CommentId);
            comment!.LikeCount--;
            await _commentRepository.SaveChangesAsync();
        }
    }
}
