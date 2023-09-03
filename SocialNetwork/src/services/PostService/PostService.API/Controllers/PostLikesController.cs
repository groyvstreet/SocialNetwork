using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Interfaces.PostLikeInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostLikesController : ControllerBase
    {
        private readonly IPostLikeService postLikeService;

        public PostLikesController(IPostLikeService postLikeService)
        {
            this.postLikeService = postLikeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            var postLike = await postLikeService.AddPostLikeAsync(addRemovePostLikeDTO);

            return Ok(postLike);
        }

        [HttpDelete]
        public async Task<IActionResult> RemovePostLikeAsync(AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            await postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO);

            return Ok();
        }
    }
}
