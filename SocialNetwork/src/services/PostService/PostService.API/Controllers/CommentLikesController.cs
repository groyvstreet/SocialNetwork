using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Interfaces.CommentsUserProfileInterfaces;

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
        public async Task<IActionResult> AddCommentLikeAsync(AddCommentLikeDTO addCommentLikeDTO)
        {
            var commentLike = await commentLikeService.AddCommentLikeAsync(addCommentLikeDTO);

            return Ok(commentLike);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveCommentLikeByIdAsync(Guid id)
        {
            await commentLikeService.RemoveCommentLikeByIdAsync(id);

            return Ok();
        }
    }
}
