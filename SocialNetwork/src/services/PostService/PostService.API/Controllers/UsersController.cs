using Microsoft.AspNetCore.Mvc;
using PostService.Application.Interfaces.UserInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("/api/comments/{id}/likes/users")]
        public async Task<IActionResult> GetUsersLikedByCommentIdIdAsync(Guid id)
        {
            var users = await userService.GetUsersLikedByCommentIdAsync(id);

            return Ok(users);
        }

        [HttpGet]
        [Route("/api/posts/{id}/likes/users")]
        public async Task<IActionResult> GetUsersLikedByPostIdIdAsync(Guid id)
        {
            var users = await userService.GetUsersLikedByPostIdAsync(id);

            return Ok(users);
        }
    }
}
