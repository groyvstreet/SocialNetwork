using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        /*public async Task Send(string message)
        {
            await Clients.All.SendAsync("Send", message);
        }*/

        public async Task SendToUser(string userId, Dialog dialog)
        {
            await Clients.User(userId).SendToUser(userId, dialog);
        }

        public async Task UpdateMessage(string userId, Dialog dialog)
        {
            await Clients.User(userId).UpdateMessage(userId, dialog);
        }

        public async Task RemoveMessage(string userId, Dialog dialog)
        {
            await Clients.User(userId).RemoveMessage(userId, dialog);
        }

        public async Task RemoveDialog(string userId, Dialog dialog)
        {
            await Clients.User(userId).RemoveDialog(userId, dialog);
        }
    }
}
