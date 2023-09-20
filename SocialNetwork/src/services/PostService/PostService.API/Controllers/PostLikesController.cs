using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Interfaces.PostLikeInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/post-likes")]
    [ApiController]
    public class PostLikesController : ControllerBase
    {
        private readonly IPostLikeService _postLikeService;

        public PostLikesController(IPostLikeService postLikeService)
        {
            _postLikeService = postLikeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPostLikeAsync([FromBody] AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            var postLike = await _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO);

            return Ok(postLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemovePostLikeAsync([FromBody] AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            await _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO);

            return NoContent();
        }
    }
}
