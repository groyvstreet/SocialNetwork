using AutoMapper;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentsUserProfileInterfaces;
using PostService.Application.Interfaces.UserProfileInterfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class CommentLikeService : ICommentLikeService
    {
        private readonly IMapper mapper;
        private readonly ICommentLikeRepository commentLikeRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IUserProfileRepository userProfileRepository;

        public CommentLikeService(IMapper mapper,
                                          ICommentLikeRepository commentLikeRepository,
                                          ICommentRepository commentRepository,
                                          IUserProfileRepository userProfileRepository)
        {
            this.mapper = mapper;
            this.commentLikeRepository = commentLikeRepository;
            this.commentRepository = commentRepository;
            this.userProfileRepository = userProfileRepository;
        }

        public async Task<GetCommentLikeDTO> AddCommentLikeAsync(AddCommentLikeDTO addCommentLikeDTO)
        {
            var comment = await commentRepository.GetCommentByIdAsync(addCommentLikeDTO.CommentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {addCommentLikeDTO.CommentId}");
            }

            var userProfile = await userProfileRepository.GetUserProfileByIdAsync(addCommentLikeDTO.UserProfileId);

            if (userProfile is null)
            {
                throw new NotFoundException($"no such user profile with id = {addCommentLikeDTO.UserProfileId}");
            }

            var commentLike = await commentLikeRepository
                .GetCommentLikeByCommentIdAndUserProfileIdAsync(addCommentLikeDTO.CommentId, addCommentLikeDTO.UserProfileId);

            if (commentLike is not null)
            {
                throw new AlreadyExistsException($"comment like with commentId = {addCommentLikeDTO.CommentId} and userProfileId = {addCommentLikeDTO.UserProfileId} already exists");
            }

            commentLike = mapper.Map<CommentLike>(addCommentLikeDTO);
            await commentLikeRepository.AddCommentLikeAsync(commentLike);
            var getCommentsUserProfileDTO = mapper.Map<GetCommentLikeDTO>(commentLike);

            comment.LikeCount++;
            await commentRepository.UpdateCommentAsync(comment);

            return getCommentsUserProfileDTO;
        }

        public async Task RemoveCommentLikeByIdAsync(Guid id)
        {
            var commentLike = await commentLikeRepository.GetCommentLikeByIdAsync(id);

            if (commentLike is null)
            {
                throw new NotFoundException($"no such comment like with id = {id}");
            }

            await commentLikeRepository.RemoveCommentLikeAsync(commentLike);

            var comment = await commentRepository.GetCommentByIdAsync(commentLike.CommentId);
            comment!.LikeCount--;
            await commentRepository.UpdateCommentAsync(comment);
        }
    }
}
