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
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetChatsByUserId(Guid userId)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var query = new GetChatsQuery(userId, Guid.Parse(authenticatedUserId));
            var dialogs = await _mediator.Send(query);
            
            return Ok(dialogs);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddChatAsync([FromBody] AddChatDTO addChatDTO)
        {
            var command = new AddChatCommand(addChatDTO);
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateChatAsync([FromBody] UpdateChatDTO updateChatDTO)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var command = new UpdateChatCommand(updateChatDTO, Guid.Parse(authenticatedUserId));
            await _mediator.Send(command);

            return Ok();
        }
    }
}
