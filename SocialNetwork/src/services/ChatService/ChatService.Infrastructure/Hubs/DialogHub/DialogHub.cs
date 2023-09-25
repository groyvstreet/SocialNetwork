using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Infrastructure.Hubs.DialogHub
{
    [Authorize]
    public class DialogHub : Hub<IDialogHub>
    {
    }
}
