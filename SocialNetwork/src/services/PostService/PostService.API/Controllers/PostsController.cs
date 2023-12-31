﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.API.Extensions;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Interfaces.PostInterfaces;

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
            var post = await _postService.AddPostAsync(addPostDTO, User.AuthenticatedUserId());

            return Ok(post);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePostAsync([FromBody] UpdatePostDTO updatePostDTO)
        {
            var post = await _postService.UpdatePostAsync(updatePostDTO, User.AuthenticatedUserId());

            return Ok(post);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> RemovePostByIdAsync(Guid id)
        {
            await _postService.RemovePostByIdAsync(id, User.AuthenticatedUserId());

            return NoContent();
        }
    }
}
