using AutoMapper;
using PostService.Application.DTOs.UserProfileDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentsUserProfileInterfaces;
using PostService.Application.Interfaces.UserProfileInterfaces;

namespace PostService.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IMapper mapper;
        private readonly IUserProfileRepository userProfileRepository;
        private readonly ICommentRepository commentRepository;
        private readonly ICommentLikeRepository commentLikeRepository;

        public UserProfileService(IMapper mapper,
                                  IUserProfileRepository userProfileRepository,
                                  ICommentRepository commentRepository,
                                  ICommentLikeRepository commentLikeRepository)
        {
            this.mapper = mapper;
            this.userProfileRepository = userProfileRepository;
            this.commentRepository = commentRepository;
            this.commentLikeRepository = commentLikeRepository;
        }

        public async Task<List<GetUserDTO>> GetUserProfilesLikedByCommentIdAsync(Guid commentId)
        {
            var comment = await commentRepository.GetCommentByIdAsync(commentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {commentId}");
            }

            var commentLikes = await commentLikeRepository.GetCommentLikesByCommentIdAsync(commentId);
            var userProfiles = commentLikes.Select(up => userProfileRepository.GetUserProfileByIdAsync(up.UserProfileId).Result);
            var getUserProfileDTOs = userProfiles.Select(mapper.Map<GetUserDTO>).ToList();

            return getUserProfileDTOs;
        }
    }
}
