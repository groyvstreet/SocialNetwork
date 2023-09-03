using Microsoft.AspNetCore.Mvc;
using PostService.Application.Interfaces.UserInterfaces;

namespace PostService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("/api/Comments/{id}/Likes/Users")]
        public async Task<IActionResult> GetUsersLikedByCommentIdIdAsync(Guid id)
        {
            var users = await userService.GetUsersLikedByCommentIdAsync(id);

            return Ok(users);
        }

        [HttpGet]
        [Route("/api/Posts/{id}/Likes/Users")]
        public async Task<IActionResult> GetUsersLikedByPostIdIdAsync(Guid id)
        {
            var users = await userService.GetUsersLikedByPostIdAsync(id);

            return Ok(users);
        }
    }
}
