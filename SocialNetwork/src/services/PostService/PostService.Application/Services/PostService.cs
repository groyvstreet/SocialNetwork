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
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPostLikeRepository _postLikeRepository;

        public PostService(IMapper mapper,
                           IPostRepository postRepository,
                           IUserRepository userRepository,
                           IPostLikeRepository postLikeRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _postLikeRepository = postLikeRepository;
        }

        public async Task<List<GetPostDTO>> GetPostsAsync()
        {
            var posts = await _postRepository.GetPostsAsync();
            var getPostDTOs = posts.Select(_mapper.Map<GetPostDTO>).ToList();

            return getPostDTOs;
        }

        public async Task<GetPostDTO> GetPostByIdAsync(Guid id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {id}");
            }

            var getPostDTO = _mapper.Map<GetPostDTO>(post);

            return getPostDTO;
        }

        public async Task<List<GetPostDTO>> GetPostsByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var posts = await _postRepository.GetPostsByUserIdAsync(userId);
            var getPostDTOs = posts.Select(_mapper.Map<GetPostDTO>).ToList();

            return getPostDTOs;
        }

        public async Task<List<GetPostDTO>> GetLikedPostsByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {userId}");
            }

            var postLikes = await _postLikeRepository.GetPostLikesByUserIdAsync(userId);
            var posts = postLikes.Select(pl => _postRepository.GetPostByIdAsync(pl.PostId).Result);
            var getPostDTOs = posts.Select(_mapper.Map<GetPostDTO>).ToList();

            return getPostDTOs;
        }

        public async Task<GetPostDTO> AddPostAsync(AddPostDTO addPostDTO)
        {
            var user = await _userRepository.GetUserByIdAsync(addPostDTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {addPostDTO.UserId}");
            }

            var post = _mapper.Map<Post>(addPostDTO);
            post.DateTime = DateTimeOffset.Now;
            await _postRepository.AddPostAsync(post);
            var getPostDTO = _mapper.Map<GetPostDTO>(post);
            
            return getPostDTO;
        }

        public async Task<GetPostDTO> UpdatePostAsync(UpdatePostDTO updatePostDTO)
        {
            var post = await _postRepository.GetPostByIdAsync(updatePostDTO.Id);
            
            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {updatePostDTO.Id}");
            }

            post.Text = updatePostDTO.Text;
            await _postRepository.UpdatePostAsync(post);
            var getPostDTO = _mapper.Map<GetPostDTO>(post);

            return getPostDTO;
        }

        public async Task RemovePostByIdAsync(Guid id)
        {
            var post = await _postRepository.GetPostByIdAsync(id);

            if (post is null)
            {
                throw new NotFoundException($"no such post with id = {id}");
            }

            await _postRepository.RemovePostAsync(post);
        }
    }
}
