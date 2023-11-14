using ChatService.API.Extensions;
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
            var command = new AddDialogMessageCommand(addDialogMessageDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut]
        [Route("/api/dialogs/messages")]
        public async Task<IActionResult> UpdateDialogMessage([FromBody] UpdateDialogMessageDTO updateDialogMessageDTO)
        {
            var command = new UpdateDialogMessageCommand(updateDialogMessageDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/dialogs/messages")]
        public async Task<IActionResult> RemoveDialogMessageAsync([FromBody] RemoveDialogMessageDTO removeDialogMessageDTO)
        {
            var command = new RemoveDialogMessageCommand(removeDialogMessageDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/dialogs/users/messages")]
        public async Task<IActionResult> RemoveDialogMessageFromUserAsync([FromBody] RemoveDialogMessageFromUserDTO removeDialogMessageFromUserDTO)
        {
            var command = new RemoveDialogMessageFromUserCommand(removeDialogMessageFromUserDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> AddChatMessageAsync([FromBody] AddChatMessageDTO addChatMessageDTO)
        {
            var command = new AddChatMessageCommand(addChatMessageDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> UpdateChatMessageAsync([FromBody] UpdateChatMessageDTO updateChatMessageDTO)
        {
            var command = new UpdateChatMessageCommand(updateChatMessageDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> RemoveChatMessageAsync([FromBody] RemoveChatMessageDTO removeChatMessageDTO)
        {
            var command = new RemoveChatMessageCommand(removeChatMessageDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/chats/users/messages")]
        public async Task<IActionResult> RemoveChatMessageFromUserAsync([FromBody] RemoveChatMessageFromUserDTO removeChatMessageFromUserDTO)
        {
            var command = new RemoveChatMessageFromUserCommand(removeChatMessageFromUserDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
