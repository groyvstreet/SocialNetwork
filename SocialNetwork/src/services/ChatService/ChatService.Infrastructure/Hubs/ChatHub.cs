using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Infrastructure.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Send", message);
        }

        public async Task SendToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("SendToUser", userId, message);
        }
    }
}
