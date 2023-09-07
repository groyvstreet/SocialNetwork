using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Interfaces;
using IdentityService.PL.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;
        private readonly IAuthorizationService authorizationService;

        public IdentityController(IIdentityService identityService,
                                  IAuthorizationService authorizationService)
        {
            this.identityService = identityService;
            this.authorizationService = authorizationService;
        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> SignUpAsync(AddUserDTO addUserDTO)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(User, addUserDTO.Role, Operations.Create);

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var user = await identityService.SignUpAsync(addUserDTO);

            return Ok(user);
        }

        [HttpPost]
        [Route("Signin")]
        public async Task<IActionResult> SignInAsync([FromQuery] string email, [FromQuery] string password)
        {
            var tokens = await identityService.SignInAsync(email, password);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> RefreshAsync([FromQuery] string accsessToken, [FromQuery] string refreshToken)
        {
            var tokens = await identityService.RefreshTokenAsync(accsessToken, refreshToken);

            return Ok(tokens);
        }
    }
}
