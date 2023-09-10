using AutoMapper;
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
            var comment = await _commentRepository.GetCommentByIdAsync(addCommentLikeDTO.CommentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {addCommentLikeDTO.CommentId}");
            }

            var user = await _userRepository.GetUserByIdAsync(addCommentLikeDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addCommentLikeDTO.UserId}");
            }

            var commentLike = await _commentLikeRepository
                .GetCommentLikeByCommentIdAndUserIdAsync(addCommentLikeDTO.CommentId, addCommentLikeDTO.UserId);

            if (commentLike is not null)
            {
                throw new AlreadyExistsException($"comment like with commentId = {addCommentLikeDTO.CommentId} and userId = {addCommentLikeDTO.UserId} already exists");
            }

            commentLike = _mapper.Map<CommentLike>(addCommentLikeDTO);
            await _commentLikeRepository.AddCommentLikeAsync(commentLike);
            var getCommentsUserDTO = _mapper.Map<GetCommentLikeDTO>(commentLike);

            comment.LikeCount++;
            await _commentRepository.UpdateCommentAsync(comment);

            return getCommentsUserDTO;
        }

        public async Task RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            var commentLike = await _commentLikeRepository.GetCommentLikeByCommentIdAndUserIdAsync(addRemoveCommentLikeDTO.CommentId, addRemoveCommentLikeDTO.UserId);

            if (commentLike is null)
            {
                throw new NotFoundException($"no such comment like with commentId = {addRemoveCommentLikeDTO.CommentId} and userId = {addRemoveCommentLikeDTO.UserId}");
            }

            await _commentLikeRepository.RemoveCommentLikeAsync(commentLike);

            var comment = await _commentRepository.GetCommentByIdAsync(commentLike.CommentId);
            comment!.LikeCount--;
            await _commentRepository.UpdateCommentAsync(comment);
        }
    }
}
