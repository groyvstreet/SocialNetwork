using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Interfaces;
using PostService.Domain.Entities;

namespace PostService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService postService;
        private readonly IRepository<UserProfile> rep;

        public PostsController(IPostService postService,
                               IRepository<UserProfile> rep)
        {
            this.postService = postService;
            this.rep = rep;
        }

        [HttpGet]
        public async Task<IActionResult> GetPostsAsync()
        {

            var posts = await postService.GetPostsAsync();

            return Ok(await rep.GetAsync());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPostByIdAsync(Guid id)
        {
            var post = await postService.GetPostByIdAsync(id);

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> AddPostAsync(AddPostDTO addPostDTO)
        {
            await postService.AddPostAsync(addPostDTO);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAlbumAsync(UpdatePostDTO updatePostDTO)
        {
            var response = await postService.UpdatePostAsync(updatePostDTO);

            if (response)
            {
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveAlbumByIdAsync(Guid id)
        {
            var response = await postService.RemovePostByIdAsync(id);

            if (response)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
