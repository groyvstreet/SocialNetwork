using ChatService.Application.Commands.ChatCommands.AddChatCommand;
using ChatService.Application.Commands.ChatCommands.UpdateChatCommand;
using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Application.Queries.ChatQueries.GetChatsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> GetChatsByUserIdAsync(Guid userId)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var query = new GetChatsQuery(userId, Guid.Parse(authenticatedUserId));
            var dialogs = await _mediator.Send(query);
            
            return Ok(dialogs);
        }

        [HttpPost]
        public async Task<IActionResult> AddChatAsync([FromBody] AddChatDTO addChatDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new AddChatCommand(addChatDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChatAsync([FromBody] UpdateChatDTO updateChatDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new UpdateChatCommand(updateChatDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
