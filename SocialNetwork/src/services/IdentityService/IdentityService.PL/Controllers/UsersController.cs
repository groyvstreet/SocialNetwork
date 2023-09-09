using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityService.PL.Controllers
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
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await userService.GetUsersAsync();

            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var user = await userService.GetUserByIdAsync(id);

            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var authenticatedUserRole = User.FindFirstValue(ClaimTypes.Role)!;

            var user = await userService.UpdateUserAsync(updateUserDTO, authenticatedUserId, authenticatedUserRole);

            return Ok(user);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveUserByIdAsync(string id)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var authenticatedUserRole = User.FindFirstValue(ClaimTypes.Role)!;

            await userService.RemoveUserByIdAsync(id, authenticatedUserId, authenticatedUserRole);

            return Ok();
        }
    }
}
