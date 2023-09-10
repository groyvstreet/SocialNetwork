using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Interfaces.CommentLikeInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/comment-likes")]
    [ApiController]
    public class CommentLikesController : ControllerBase
    {
        private readonly ICommentLikeService _commentLikeService;

        public CommentLikesController(ICommentLikeService commentLikeService)
        {
            _commentLikeService = commentLikeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            var commentLike = await _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO);

            return Ok(commentLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            await _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO);

            return Ok();
        }
    }
}
