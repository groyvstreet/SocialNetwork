using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Interfaces;
using IdentityService.PL.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.PL.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthorizationService authorizationService;

        public UsersController(IUserService userService,
                               IAuthorizationService authorizationService)
        {
            this.userService = userService;
            this.authorizationService = authorizationService;
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
            var authorizationResult = await authorizationService.AuthorizeAsync(User, updateUserDTO.Id, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var user = await userService.UpdateUserAsync(updateUserDTO);

            return Ok(user);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveUserByIdAsync(string id)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            await userService.RemoveUserByIdAsync(id);

            return Ok();
        }
    }
}
