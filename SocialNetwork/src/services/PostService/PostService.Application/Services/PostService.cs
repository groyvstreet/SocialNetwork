using AutoMapper;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.UserProfileInterfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IMapper mapper;
        private readonly IPostRepository postRepository;
        private readonly IUserProfileRepository userProfileRepository;

        public PostService(IMapper mapper,
                           IPostRepository postRepository,
                           IUserProfileRepository userProfileRepository)
        {
            this.mapper = mapper;
            this.postRepository = postRepository;
            this.userProfileRepository = userProfileRepository;
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

        public async Task<List<GetPostDTO>> GetPostsByUserProfileIdAsync(Guid userProfileId)
        {
            var userProfile = await userProfileRepository.GetUserProfileByIdAsync(userProfileId);

            if (userProfile is null)
            {
                throw new NotFoundException($"no such user profile with id = {userProfileId}");
            }

            var posts = await postRepository.GetPostsByUserProfileIdAsync(userProfileId);
            var getPostDTOs = posts.Select(mapper.Map<GetPostDTO>).ToList();

            return getPostDTOs;
        }

        public async Task<GetPostDTO> AddPostAsync(AddPostDTO addPostDTO)
        {
            var userProfile = await userProfileRepository.GetUserProfileByIdAsync(addPostDTO.UserProfileId);

            if (userProfile is null)
            {
                throw new NotFoundException($"no such user profile with id = {addPostDTO.UserProfileId}");
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
