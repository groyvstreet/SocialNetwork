using ChatService.Application.Commands.ChatCommands.AddUserToChatCommand;
using ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand;
using ChatService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserRepository _userRepository;

        public UsersController(IMediator mediator,
                               IUserRepository userRepository)
        {
            _mediator = mediator;
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
    }
}
