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
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await _userService.GetUsersAsync();

            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDTO updateUserDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var authenticatedUserRole = User.FindFirstValue(ClaimTypes.Role)!;

            var user = await _userService.UpdateUserAsync(updateUserDTO, authenticatedUserId, authenticatedUserRole);

            return Ok(user);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveUserByIdAsync(string id)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var authenticatedUserRole = User.FindFirstValue(ClaimTypes.Role)!;

            await _userService.RemoveUserByIdAsync(id, authenticatedUserId, authenticatedUserRole);

            return NoContent();
        }
    }
}
