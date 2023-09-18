using ChatService.Application.Queries.DialogQueries.GetDialogsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers
{
    [Route("api/dialogs")]
    [ApiController]
    public class DialogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DialogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDialogsByUserId([FromQuery] GetDialogsQuery query)
        {
            var dialogs = await _mediator.Send(query);

            return Ok(dialogs);
        }
    }
}
