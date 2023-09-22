using ChatService.Application;
using ChatService.Application.Commands.ChatCommands.AddUserToChatCommand;
using ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand;
using ChatService.Application.Interfaces.Repositories;
using MediatR;
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
        private readonly IUserRepository _userRepository;

        public UsersController(IMediator mediator,
                               IOptions<JwtOptions> jwtOptions,
                               IUserRepository userRepository)
        {
            _mediator = mediator;
            _jwtOptions = jwtOptions;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("/api/chats/users")]
        public async Task<IActionResult> AddUserToChatAsync([FromBody] AddUserToChatCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/chats/users")]
        public async Task<IActionResult> RemoveUserFromChatAsync([FromBody] RemoveUserFromChatCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost]
        [Route("/api/chats/admins")]
        public async Task<IActionResult> SetUserAsChatAdminAsync([FromBody] SetUserAsChatAdminCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/chats/admins")]
        public async Task<IActionResult> SetUserAsDefaultAsync([FromBody] SetUserAsDefaultCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();

            return Ok(users);
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
