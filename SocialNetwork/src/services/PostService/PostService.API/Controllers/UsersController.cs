using Microsoft.AspNetCore.Mvc;
using PostService.Application.Interfaces.UserInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("/api/comments/{id}/likes/users")]
        public async Task<IActionResult> GetUsersLikedByCommentIdIdAsync(Guid id)
        {
            var users = await _userService.GetUsersLikedByCommentIdAsync(id);

            return Ok(users);
        }

        [HttpGet]
        [Route("/api/posts/{id}/likes/users")]
        public async Task<IActionResult> GetUsersLikedByPostIdIdAsync(Guid id)
        {
            var users = await _userService.GetUsersLikedByPostIdAsync(id);

            return Ok(users);
        }
    }
}
