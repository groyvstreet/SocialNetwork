using ChatService.Application.Commands.ChatCommands.AddChatMessageCommand;
using ChatService.Application.Commands.ChatCommands.RemoveChatMessageCommand;
using ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand;
using ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand;
using ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand;
using ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand;
using ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand;
using ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand;
using ChatService.Application.DTOs.MessageDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatService.API.Controllers
{
    [Route("api/messages")]
    [Authorize]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/api/dialogs/messages")]
        public async Task<IActionResult> AddDialogMessageAsync([FromBody] AddDialogMessageDTO addDialogMessageDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new AddDialogMessageCommand(addDialogMessageDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut]
        [Route("/api/dialogs/messages")]
        public async Task<IActionResult> UpdateDialogMessage([FromBody] UpdateDialogMessageDTO updateDialogMessageDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new UpdateDialogMessageCommand(updateDialogMessageDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/dialogs/messages")]
        public async Task<IActionResult> RemoveDialogMessageAsync([FromBody] RemoveDialogMessageDTO removeDialogMessageDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new RemoveDialogMessageCommand(removeDialogMessageDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/dialogs/users/messages")]
        public async Task<IActionResult> RemoveDialogMessageFromUserAsync([FromBody] RemoveDialogMessageFromUserDTO removeDialogMessageFromUserDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new RemoveDialogMessageFromUserCommand(removeDialogMessageFromUserDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> AddChatMessageAsync([FromBody] AddChatMessageDTO addChatMessageDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new AddChatMessageCommand(addChatMessageDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> UpdateChatMessageAsync([FromBody] UpdateChatMessageDTO updateChatMessageDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new UpdateChatMessageCommand(updateChatMessageDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> RemoveChatMessageAsync([FromBody] RemoveChatMessageDTO removeChatMessageDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new RemoveChatMessageCommand(removeChatMessageDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/chats/users/messages")]
        public async Task<IActionResult> RemoveChatMessageFromUserAsync([FromBody] RemoveChatMessageFromUserDTO removeChatMessageFromUserDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new RemoveChatMessageFromUserCommand(removeChatMessageFromUserDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
