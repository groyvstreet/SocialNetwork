using ChatService.API.Extensions;
using ChatService.Application.Commands.ChatCommands.AddChatCommand;
using ChatService.Application.Commands.ChatCommands.UpdateChatCommand;
using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Application.Queries.ChatQueries.GetChatsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers
{
    [Route("api/chats")]
    [Authorize]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsByUserId(Guid userId)
        {
            var query = new GetChatsQuery(userId, User.AuthenticatedUserId());
            var dialogs = await _mediator.Send(query);
            
            return Ok(dialogs);
        }

        [HttpPost]
        public async Task<IActionResult> AddChatAsync([FromBody] AddChatDTO addChatDTO)
        {
            var command = new AddChatCommand(addChatDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChatAsync([FromBody] UpdateChatDTO updateChatDTO)
        {
            var command = new UpdateChatCommand(updateChatDTO, User.AuthenticatedUserId());
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
