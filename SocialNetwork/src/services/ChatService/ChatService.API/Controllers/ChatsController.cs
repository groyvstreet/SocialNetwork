using ChatService.Application.Commands.ChatCommands.AddChatCommand;
using ChatService.Application.Commands.ChatCommands.UpdateChatCommand;
using ChatService.Application.Queries.ChatQueries.GetChatsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetChatsByUserId([FromQuery] GetChatsQuery query)
        {
            var dialogs = await _mediator.Send(query);

            return Ok(dialogs);
        }

        [HttpPost]
        public async Task<IActionResult> AddChatAsync([FromBody] AddChatCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChatAsync([FromBody] UpdateChatCommand command)
        {
            await _mediator.Send(command);

            return Ok();
        }
    }
}
