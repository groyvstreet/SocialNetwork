using AutoMapper;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Interfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Post> postRepository;

        public PostService(IMapper mapper,
                           IRepository<Post> postRepository)
        {
            this.mapper = mapper;
            this.postRepository = postRepository;
        }

        public async Task<List<GetPostDTO>> GetPostsAsync()
        {
            var posts = await postRepository.GetAsync();
            var postDTOs = posts.Select(mapper.Map<GetPostDTO>).ToList();

            return postDTOs;
        }

        public async Task<GetPostDTO?> GetPostByIdAsync(Guid id)
        {
            var post = await postRepository.GetFirstOrDefaultByAsync(p => p.Id == id);
            var postDTO = mapper.Map<GetPostDTO>(post);

            return postDTO;
        }

        public async Task AddPostAsync(AddPostDTO addPostDTO)
        {
            var post = mapper.Map<Post>(addPostDTO);
            await postRepository.AddAsync(post);
        }

        public async Task<bool> UpdatePostAsync(UpdatePostDTO updatePostDTO)
        {
            var post = await postRepository.GetFirstOrDefaultByAsync(p => p.Id == updatePostDTO.Id);
            
            if (post == null)
            {
                return false;
            }

            post.Text = updatePostDTO.Text;
            await postRepository.UpdateAsync(post);

            return true;
        }

        public async Task<bool> RemovePostByIdAsync(Guid id)
        {
            var post = await postRepository.GetFirstOrDefaultByAsync(p => p.Id == id);

            if (post == null)
            {
                return false;
            }

            await postRepository.RemoveAsync(post);

            return true;
        }
    }
}
