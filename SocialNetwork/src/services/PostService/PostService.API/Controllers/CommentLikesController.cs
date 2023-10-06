using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.API.Extensions;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Interfaces.CommentLikeInterfaces;

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
            var commentLike = await _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, User.AuthenticatedUserId());

            return Ok(commentLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCommentLikeAsync([FromBody] AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            await _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, User.AuthenticatedUserId());

            return NoContent();
        }
    }
}
