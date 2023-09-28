using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using System.Security.Claims;

namespace PostService.API.Controllers
{
    [Route("api/comment-likes")]
    [Authorize]
    [ApiController]
    public class CommentLikesController : ControllerBase
    {
        private readonly ICommentLikeService _commentLikeService;

        public CommentLikesController(ICommentLikeService commentLikeService)
        {
            _commentLikeService = commentLikeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentLikeAsync([FromBody] AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var commentLike = await _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            return Ok(commentLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCommentLikeAsync([FromBody] AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            return NoContent();
        }
    }
}
