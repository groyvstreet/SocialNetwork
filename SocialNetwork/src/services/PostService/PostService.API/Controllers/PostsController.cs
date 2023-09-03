using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Interfaces.PostInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService postService;

        public PostsController(IPostService postService)
        {
            this.postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPostsAsync()
        {

            var posts = await postService.GetPostsAsync();

            return Ok(posts);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPostByIdAsync(Guid id)
        {
            var post = await postService.GetPostByIdAsync(id);

            return Ok(post);
        }

        [HttpGet]
        [Route("/api/UserProfiles/{id}/Posts")]
        public async Task<IActionResult> GetPostsByUserProfileIdAsync(Guid id)
        {
            var posts = await postService.GetPostsByUserProfileIdAsync(id);

            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> AddPostAsync(AddPostDTO addPostDTO)
        {
            var post = await postService.AddPostAsync(addPostDTO);

            return Ok(post);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePostAsync(UpdatePostDTO updatePostDTO)
        {
            var post = await postService.UpdatePostAsync(updatePostDTO);

            return Ok(post);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemovePostByIdAsync(Guid id)
        {
            await postService.RemovePostByIdAsync(id);

            return Ok();
        }
    }
}
