using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.PL.Controllers
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
        public async Task<IActionResult> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            var user = await userService.UpdateUserAsync(updateUserDTO);

            return Ok(user);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveUserByIdAsync(string id)
        {
            await userService.RemoveUserByIdAsync(id);

            return Ok();
        }
    }
}
