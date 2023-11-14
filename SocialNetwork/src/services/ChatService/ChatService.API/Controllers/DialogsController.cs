using ChatService.API.Extensions;
using ChatService.Application.Queries.DialogQueries.GetDialogsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.API.Controllers
{
    [Route("api/dialogs")]
    [Authorize]
    [ApiController]
    public class DialogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DialogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetDialogsByUserId(Guid userId)
        {
            var query = new GetDialogsQuery(userId, User.AuthenticatedUserId());
            var dialogs = await _mediator.Send(query);

            return Ok(dialogs);
        }
    }
}
