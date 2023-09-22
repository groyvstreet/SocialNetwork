using ChatService.Application.Interfaces.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Hubs
{
    [Authorize]
    public class DialogHub : Hub<IDialogHub>
    {
    }
}
