using ChatService.Application.Commands.ChatCommands.AddChatMessageCommand;
using ChatService.Application.Commands.ChatCommands.RemoveChatMessageCommand;
using ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand;
using ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand;
using ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand;
using ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand;
using ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand;
using ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> AddDialogMessageAsync([FromBody] AddDialogMessageCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPut]
        [Route("/api/dialogs/messages")]
        public async Task<IActionResult> UpdateDialogMessage([FromBody] UpdateDialogMessageCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/dialogs/messages")]
        public async Task<IActionResult> RemoveDialogMessageAsync([FromBody] RemoveDialogMessageCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/dialogs/users/messages")]
        public async Task<IActionResult> RemoveDialogMessageFromUserAsync([FromBody] RemoveDialogMessageFromUserCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> AddChatMessageAsync([FromBody] AddChatMessageCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPut]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> UpdateChatMessageAsync([FromBody] UpdateChatMessageCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpDelete]
        [Route("/api/chats/messages")]
        public async Task<IActionResult> RemoveChatMessageAsync([FromBody] RemoveChatMessageCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [Route("/api/chats/users/messages")]
        public async Task<IActionResult> RemoveChatMessageFromUserAsync([FromBody] RemoveChatMessageFromUserCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
