using ChatService.Application.Commands.MessageCommands;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> AddMessageToDialogAsync([FromBody] AddDialogMessageCommand command)
        {
            var message = await _mediator.Send(command);

            return Ok(message);
        }
    }
}
