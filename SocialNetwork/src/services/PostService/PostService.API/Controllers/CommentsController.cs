using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.Interfaces.CommentInterfaces;
using System.Security.Claims;

namespace PostService.API.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsAsync()
        {
            var comments = await _commentService.GetCommentsAsync();

            return Ok(comments);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCommentByIdAsync(Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);

            return Ok(comment);
        }

        [HttpGet]
        [Route("/api/posts/{id}/comments")]
        public async Task<IActionResult> GetCommentsByPostIdAsync(Guid id)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(id);

            return Ok(comments);
        }

        [HttpGet]
        [Route("/api/users/{id}/comment-likes/comments")]
        public async Task<IActionResult> GetLikedCommentsByUserIdAsync(Guid id)
        {
            var comments = await _commentService.GetLikedCommentsByUserIdAsync(id);

            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCommentAsync([FromBody] AddCommentDTO addCommentDTO)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var comment = await _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId);

            return Ok(comment);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateCommentAsync([FromBody] UpdateCommentDTO updateCommentDTO)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var comment = await _commentService.UpdateCommentAsync(updateCommentDTO, authenticatedUserId);

            return Ok(comment);
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public async Task<IActionResult> RemoveCommentByIdAsync(Guid id)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentService.RemoveCommentByIdAsync(id, authenticatedUserId);

            return NoContent();
        }
    }
}
