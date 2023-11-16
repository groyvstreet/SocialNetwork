using ChatService.Application.Queries.DialogQueries.GetDialogsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> GetDialogsByUserIdAsync(Guid userId)
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var query = new GetDialogsQuery(userId, Guid.Parse(authenticatedUserId));
            var dialogs = await _mediator.Send(query);

            return Ok(dialogs);
        }
    }
}
