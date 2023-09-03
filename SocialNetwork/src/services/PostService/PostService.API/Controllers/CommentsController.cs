using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.Interfaces.CommentInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService commentService;

        public CommentsController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsAsync()
        {

            var comments = await commentService.GetCommentsAsync();

            return Ok(comments);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetCommentByIdAsync(Guid id)
        {
            var comment = await commentService.GetCommentByIdAsync(id);

            return Ok(comment);
        }

        [HttpGet]
        [Route("/api/Posts/{id}/Comments")]
        public async Task<IActionResult> GetCommentsByPostIdAsync(Guid id)
        {
            var comments = await commentService.GetCommentsByPostIdAsync(id);

            return Ok(comments);
        }

        [HttpGet]
        [Route("/api/UserProfiles/{id}/CommentLikes/Comments")]
        public async Task<IActionResult> GetLikedCommentsByUserProfileIdAsync(Guid id)
        {
            var comments = await commentService.GetLikedCommentsByUserProfileIdAsync(id);

            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentAsync(AddCommentDTO addCommentDTO)
        {
            var comment = await commentService.AddCommentAsync(addCommentDTO);

            return Ok(comment);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO)
        {
            var comment = await commentService.UpdateCommentAsync(updateCommentDTO);

            return Ok(comment);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveCommentByIdAsync(Guid id)
        {
            await commentService.RemoveCommentByIdAsync(id);

            return Ok();
        }
    }
}
