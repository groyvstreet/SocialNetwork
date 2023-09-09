﻿using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.PL.Controllers
{
    [Route("api/identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;
        private readonly ITokenService tokenService;

        public IdentityController(IIdentityService identityService,
                                  ITokenService tokenService)
        {
            this.identityService = identityService;
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> SignUpAsync(AddUserDTO addUserDTO)
        {
            var user = await identityService.SignUpAsync(addUserDTO);

            return Ok(user);
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignInAsync([FromQuery] string email, [FromQuery] string password)
        {
            var tokens = await identityService.SignInAsync(email, password);

            return Ok(tokens);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshAsync([FromQuery] string accsessToken, [FromQuery] string refreshToken)
        {
            var tokens = await tokenService.RefreshTokenAsync(accsessToken, refreshToken);

            return Ok(tokens);
        }
    }
}
