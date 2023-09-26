using ChatService.Application.Commands.ChatCommands.AddUserToChatCommand;
using ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand;
using ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand;
using ChatService.Application.DTOs.ChatDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatService.API.Controllers
{
    [Route("api/users")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/api/chats/users")]
        public async Task<IActionResult> AddUserToChatAsync([FromBody] AddUserToChatDTO addUserToChatDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new AddUserToChatCommand(addUserToChatDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/chats/users")]
        public async Task<IActionResult> RemoveUserFromChatAsync([FromBody] RemoveUserFromChatDTO removeUserFromChatDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new RemoveUserFromChatCommand(removeUserFromChatDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost]
        [Route("/api/chats/admins")]
        public async Task<IActionResult> SetUserAsChatAdminAsync([FromBody] SetUserAsChatAdminDTO setUserAsChatAdminDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new SetUserAsChatAdminCommand(setUserAsChatAdminDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/chats/admins")]
        public async Task<IActionResult> SetUserAsDefaultAsync([FromBody] SetUserAsDefaultDTO setUserAsDefaultDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new SetUserAsDefaultCommand(setUserAsDefaultDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
