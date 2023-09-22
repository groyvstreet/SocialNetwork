using ChatService.Application;
using ChatService.Application.Commands.ChatCommands.AddUserToChatCommand;
using ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand;
using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatService.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public UsersController(IMediator mediator,
                               IOptions<JwtOptions> jwtOptions)
        {
            _mediator = mediator;
            _jwtOptions = jwtOptions;
        }

        [HttpPost]
        [Route("/api/chats/users")]
        [Authorize]
        public async Task<IActionResult> AddUserToChatAsync([FromBody] AddUserToChatDTO addUserToChatDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new AddUserToChatCommand(addUserToChatDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/chats/users")]
        [Authorize]
        public async Task<IActionResult> RemoveUserFromChatAsync([FromBody] RemoveUserFromChatDTO removeUserFromChatDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new RemoveUserFromChatCommand(removeUserFromChatDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost]
        [Route("/api/chats/admins")]
        [Authorize]
        public async Task<IActionResult> SetUserAsChatAdminAsync([FromBody] SetUserAsChatAdminDTO setUserAsChatAdminDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new SetUserAsChatAdminCommand(setUserAsChatAdminDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/chats/admins")]
        [Authorize]
        public async Task<IActionResult> SetUserAsDefaultAsync([FromBody] SetUserAsDefaultDTO setUserAsDefaultDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new SetUserAsDefaultCommand(setUserAsDefaultDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GetToken(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtOptions.Value.AccessTokenLifeTime)),
                signingCredentials: new SigningCredentials(_jwtOptions.Value.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
