using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using System.Text.Json;

namespace PostService.Application.Services
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly IMapper _mapper;
        private readonly ICommentLikeRepository _commentLikeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CommentLikeService> _logger;

        public CommentLikeService(IMapper mapper,
                                  ICommentLikeRepository commentLikeRepository,
                                  ICommentRepository commentRepository,
                                  IUserRepository userRepository,
                                  ILogger<CommentLikeService> logger)
        {
            _mapper = mapper;
            _commentLikeRepository = commentLikeRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<GetCommentLikeDTO> AddCommentLikeAsync(AddRemoveCommentLikeDTO addCommentLikeDTO, Guid authenticatedUserId)
        {
            if (addCommentLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var comment = await _commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == addCommentLikeDTO.CommentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {addCommentLikeDTO.CommentId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == addCommentLikeDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addCommentLikeDTO.UserId}");
            }

            var commentLike = await _commentLikeRepository.GetFirstOrDefaultByAsync(commentLike => 
                commentLike.CommentId == addCommentLikeDTO.CommentId && commentLike.UserId == addCommentLikeDTO.UserId);

            if (commentLike is not null)
            {
                throw new AlreadyExistsException($"comment like with commentId = {addCommentLikeDTO.CommentId} and userId = {addCommentLikeDTO.UserId} already exists");
            }

            commentLike = _mapper.Map<CommentLike>(addCommentLikeDTO);
            await _commentLikeRepository.AddAsync(commentLike);
            await _commentLikeRepository.SaveChangesAsync();
            var getCommentLikeDTO = _mapper.Map<GetCommentLikeDTO>(commentLike);

            comment.LikeCount++;
            await _commentRepository.SaveChangesAsync();

            _logger.LogInformation("commentLike - {commentLike} added", JsonSerializer.Serialize(commentLike));

            return getCommentLikeDTO;
        }

        public async Task RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO, Guid authenticatedUserId)
        {
            if (addRemoveCommentLikeDTO.UserId != authenticatedUserId)
            {
                throw new ForbiddenException();
            }

            var commentLike = await _commentLikeRepository.GetFirstOrDefaultByAsync(commentLike =>
                commentLike.CommentId == addRemoveCommentLikeDTO.CommentId && commentLike.UserId == addRemoveCommentLikeDTO.UserId);

            if (commentLike is null)
            {
                throw new NotFoundException($"no such comment like with commentId = {addRemoveCommentLikeDTO.CommentId} and userId = {addRemoveCommentLikeDTO.UserId}");
            }

            _commentLikeRepository.Remove(commentLike);
            await _commentLikeRepository.SaveChangesAsync();

            var comment = await _commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentLike.CommentId);
            comment!.LikeCount--;
            await _commentRepository.SaveChangesAsync();

            _logger.LogInformation("commentLike - {commentLike} removed", JsonSerializer.Serialize(commentLike));
        }
    }
}
