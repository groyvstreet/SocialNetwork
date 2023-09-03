using AutoMapper;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IMapper mapper;
        private readonly IPostRepository postRepository;
        private readonly IUserRepository userRepository;
        private readonly IPostLikeRepository postLikeRepository;

        public PostService(IMapper mapper,
                           IPostRepository postRepository,
                           IUserRepository userRepository,
                           IPostLikeRepository postLikeRepository)
        {
            this.mapper = mapper;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.postLikeRepository = postLikeRepository;
        }

        public async Task<List<GetPostDTO>> GetPostsAsync()
        {
            var posts = await postRepository.GetPostsAsync();
            var getPostDTOs = posts.Select(mapper.Map<GetPostDTO>).ToList();

            return getPostDTOs;
        }

        public async Task<GetPostDTO> GetPostByIdAsync(Guid id)
        {
            var post = await postRepository.GetPostByIdAsync(id);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {id}");
            }

            var getPostDTO = mapper.Map<GetPostDTO>(post);

            return getPostDTO;
        }

        public async Task<List<GetPostDTO>> GetPostsByUserIdAsync(Guid userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var posts = await postRepository.GetPostsByUserIdAsync(userId);
            var getPostDTOs = posts.Select(mapper.Map<GetPostDTO>).ToList();

            return getPostDTOs;
        }

        public async Task<List<GetPostDTO>> GetLikedPostsByUserIdAsync(Guid userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var postLikes = await postLikeRepository.GetPostLikesByUserIdAsync(userId);
            var posts = postLikes.Select(pl => postRepository.GetPostByIdAsync(pl.PostId).Result);
            var getPostDTOs = posts.Select(mapper.Map<GetPostDTO>).ToList();

            return getPostDTOs;
        }

        public async Task<GetPostDTO> AddPostAsync(AddPostDTO addPostDTO)
        {
            var user = await userRepository.GetUserByIdAsync(addPostDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addPostDTO.UserId}");
            }

            var post = mapper.Map<Post>(addPostDTO);
            post.DateTime = DateTimeOffset.Now;
            await postRepository.AddPostAsync(post);
            var getPostDTO = mapper.Map<GetPostDTO>(post);
            
            return getPostDTO;
        }

        public async Task<GetPostDTO> UpdatePostAsync(UpdatePostDTO updatePostDTO)
        {
            var post = await postRepository.GetPostByIdAsync(updatePostDTO.Id);
            
            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {updatePostDTO.Id}");
            }

            post.Text = updatePostDTO.Text;
            await postRepository.UpdatePostAsync(post);
            var getPostDTO = mapper.Map<GetPostDTO>(post);

            return getPostDTO;
        }

        public async Task RemovePostByIdAsync(Guid id)
        {
            var post = await postRepository.GetPostByIdAsync(id);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {id}");
            }

            await postRepository.RemovePostAsync(post);
        }
    }
}
