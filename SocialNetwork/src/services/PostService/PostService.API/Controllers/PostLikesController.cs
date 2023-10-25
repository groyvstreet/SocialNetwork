using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.API.Extensions;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Interfaces.PostLikeInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/post-likes")]
    [Authorize]
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
            var postLike = await _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, User.AuthenticatedUserId());

            return Ok(postLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemovePostLikeAsync([FromBody] AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            await _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, User.AuthenticatedUserId());

            return NoContent();
        }
    }
}
