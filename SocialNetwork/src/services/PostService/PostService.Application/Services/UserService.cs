using AutoMapper;
using PostService.Application.DTOs.UserDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;

namespace PostService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly ICommentRepository commentRepository;
        private readonly ICommentLikeRepository commentLikeRepository;
        private readonly IPostRepository postRepository;
        private readonly IPostLikeRepository postLikeRepository;

        public UserService(IMapper mapper,
                                  IUserRepository userRepository,
                                  ICommentRepository commentRepository,
                                  ICommentLikeRepository commentLikeRepository,
                                  IPostRepository postRepository,
                                  IPostLikeRepository postLikeRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.commentRepository = commentRepository;
            this.commentLikeRepository = commentLikeRepository;
            this.postRepository = postRepository;
            this.postLikeRepository = postLikeRepository;
        }

        public async Task<List<GetUserDTO>> GetUsersLikedByCommentIdAsync(Guid commentId)
        {
            var comment = await commentRepository.GetCommentByIdAsync(commentId);

            if (comment is null)
            {
                throw new NotFoundException($"no such comment with id = {commentId}");
            }

            var commentLikes = await commentLikeRepository.GetCommentLikesByCommentIdAsync(commentId);
            var users = commentLikes.Select(up => userRepository.GetUserByIdAsync(up.UserId).Result);
            var getUserDTOs = users.Select(mapper.Map<GetUserDTO>).ToList();

            return getUserDTOs;
        }

        public async Task<List<GetUserDTO>> GetUsersLikedByPostIdAsync(Guid postId)
        {
            var post = await postRepository.GetPostByIdAsync(postId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {postId}");
            }

            var postLikes = await postLikeRepository.GetPostLikesByPostIdAsync(postId);
            var users = postLikes.Select(up => userRepository.GetUserByIdAsync(up.UserId).Result);
            var getUserDTOs = users.Select(mapper.Map<GetUserDTO>).ToList();

            return getUserDTOs;
        }
    }
}
