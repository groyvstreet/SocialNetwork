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
        private readonly IMapper mapper;
        private readonly ICommentLikeRepository commentLikeRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IUserRepository userRepository;

        public CommentLikeService(IMapper mapper,
                                          ICommentLikeRepository commentLikeRepository,
                                          ICommentRepository commentRepository,
                                          IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.commentLikeRepository = commentLikeRepository;
            this.commentRepository = commentRepository;
            this.userRepository = userRepository;
        }

        public async Task<GetCommentLikeDTO> AddCommentLikeAsync(AddRemoveCommentLikeDTO addCommentLikeDTO)
        {
            var comment = await commentRepository.GetCommentByIdAsync(addCommentLikeDTO.CommentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {addCommentLikeDTO.CommentId}");
            }

            var user = await userRepository.GetUserByIdAsync(addCommentLikeDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addCommentLikeDTO.UserId}");
            }

            var commentLike = await commentLikeRepository
                .GetCommentLikeByCommentIdAndUserIdAsync(addCommentLikeDTO.CommentId, addCommentLikeDTO.UserId);

            if (commentLike is not null)
            {
                throw new AlreadyExistsException($"comment like with commentId = {addCommentLikeDTO.CommentId} and userId = {addCommentLikeDTO.UserId} already exists");
            }

            commentLike = mapper.Map<CommentLike>(addCommentLikeDTO);
            await commentLikeRepository.AddCommentLikeAsync(commentLike);
            var getCommentsUserDTO = mapper.Map<GetCommentLikeDTO>(commentLike);

            comment.LikeCount++;
            await commentRepository.UpdateCommentAsync(comment);

            return getCommentsUserDTO;
        }

        public async Task RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            var commentLike = await commentLikeRepository.GetCommentLikeByCommentIdAndUserIdAsync(addRemoveCommentLikeDTO.CommentId, addRemoveCommentLikeDTO.UserId);

            if (commentLike is null)
            {
                throw new NotFoundException($"no such comment like with commentId = {addRemoveCommentLikeDTO.CommentId} and userId = {addRemoveCommentLikeDTO.UserId}");
            }

            await commentLikeRepository.RemoveCommentLikeAsync(commentLike);

            var comment = await commentRepository.GetCommentByIdAsync(commentLike.CommentId);
            comment!.LikeCount--;
            await commentRepository.UpdateCommentAsync(comment);
        }
    }
}
