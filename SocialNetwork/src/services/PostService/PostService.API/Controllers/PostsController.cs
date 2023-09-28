using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Interfaces.PostInterfaces;
using System.Security.Claims;

namespace PostService.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPostsAsync()
        {

            var posts = await _postService.GetPostsAsync();

            return Ok(posts);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPostByIdAsync(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            return Ok(post);
        }

        [HttpGet]
        [Route("/api/users/{id}/posts")]
        public async Task<IActionResult> GetPostsByUserIdAsync(Guid id)
        {
            var posts = await _postService.GetPostsByUserIdAsync(id);

            return Ok(posts);
        }

        [HttpGet]
        [Route("/api/users/{id}/post-likes/posts")]
        public async Task<IActionResult> GetLikedPostsByUserIdAsync(Guid id)
        {
            var posts = await _postService.GetLikedPostsByUserIdAsync(id);

            return Ok(posts);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddPostAsync([FromBody] AddPostDTO addPostDTO)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = await _postService.AddPostAsync(addPostDTO, authenticatedUserId);

            return Ok(post);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePostAsync([FromBody] UpdatePostDTO updatePostDTO)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var post = await _postService.UpdatePostAsync(updatePostDTO, authenticatedUserId);

            return Ok(post);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> RemovePostByIdAsync(Guid id)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _postService.RemovePostByIdAsync(id, authenticatedUserId);

            return NoContent();
        }
    }
}
