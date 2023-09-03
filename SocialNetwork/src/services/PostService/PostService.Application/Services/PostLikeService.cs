using AutoMapper;
using PostService.Application.DTOs.CommentLikeDTOs;
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
        private readonly IMapper mapper;
        private readonly IPostLikeRepository postLikeRepository;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;

        public PostLikeService(IMapper mapper,
                               IPostLikeRepository postLikeRepository,
                               IPostRepository postRepository,
                               IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.postLikeRepository = postLikeRepository;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
        }

        public async Task<GetPostLikeDTO> AddPostLikeAsync(AddRemovePostLikeDTO addPostLikeDTO)
        {
            var post = await postRepository.GetPostByIdAsync(addPostLikeDTO.PostId);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {addPostLikeDTO.PostId}");
            }

            var user = await userRepository.GetUserByIdAsync(addPostLikeDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addPostLikeDTO.UserId}");
            }

            var postLike = await postLikeRepository
                .GetPostLikeByPostIdAndUserIdAsync(addPostLikeDTO.PostId, addPostLikeDTO.UserId);

            if (postLike is not null)
            {
                throw new AlreadyExistsException($"post like with postId = {addPostLikeDTO.PostId} and userId = {addPostLikeDTO.UserId} already exists");
            }

            postLike = mapper.Map<PostLike>(addPostLikeDTO);
            await postLikeRepository.AddPostLikeAsync(postLike);
            var getPostLikeDTO = mapper.Map<GetPostLikeDTO>(postLike);

            post.LikeCount++;
            await postRepository.UpdatePostAsync(post);

            return getPostLikeDTO;
        }

        public async Task RemovePostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            var postLike = await postLikeRepository.GetPostLikeByPostIdAndUserIdAsync(addRemovePostLikeDTO.PostId, addRemovePostLikeDTO.UserId);

            if (postLike is null)
            {
                throw new NotFoundException($"no such post like with postId = {addRemovePostLikeDTO.PostId} and userId = {addRemovePostLikeDTO.UserId}");
            }

            await postLikeRepository.RemovePostLikeAsync(postLike);

            var post = await postRepository.GetPostByIdAsync(postLike.PostId);
            post!.LikeCount--;
            await postRepository.UpdatePostAsync(post);
        }
    }
}
