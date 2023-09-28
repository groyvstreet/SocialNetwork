using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Interfaces.PostLikeInterfaces;
using System.Security.Claims;

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
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var postLike = await _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            return Ok(postLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemovePostLikeAsync([FromBody] AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            var authenticatedUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            return NoContent();
        }
    }
}
