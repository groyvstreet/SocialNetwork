using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Interfaces.CommentLikeInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentLikesController : ControllerBase
    {
        private readonly ICommentLikeService commentLikeService;

        public CommentLikesController(ICommentLikeService commentLikeService)
        {
            this.commentLikeService = commentLikeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            var commentLike = await commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO);

            return Ok(commentLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            await commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO);

            return Ok();
        }
    }
}
