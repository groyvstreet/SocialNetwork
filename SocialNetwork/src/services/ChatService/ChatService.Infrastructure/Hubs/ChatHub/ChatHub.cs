using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Infrastructure.Hubs.ChatHub
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
    }
}
