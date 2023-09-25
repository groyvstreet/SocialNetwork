using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using ChatService.Infrastructure.Hubs.DialogHub;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Infrastructure.Services
{
    public class DialogNotificationService : IDialogNotificationService
    {
        private readonly IHubContext<DialogHub, IDialogHub> _hubContext;

        public DialogNotificationService(IHubContext<DialogHub, IDialogHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageAsync(Dialog dialog, Message message)
        {
            dialog.Messages = new List<Message> { message };
            dialog.MessageCount++;
            await _hubContext.Clients.User(dialog.Users[0].Id.ToString()).SendMessageAsync(dialog);
            await _hubContext.Clients.User(dialog.Users[1].Id.ToString()).SendMessageAsync(dialog);
        }

        public async Task RemoveDialogAsync(Dialog dialog)
        {
            dialog.Messages.Clear();
            dialog.MessageCount = 0;
            await _hubContext.Clients.User(dialog.Users[0].Id.ToString()).RemoveDialogAsync(dialog);
            await _hubContext.Clients.User(dialog.Users[1].Id.ToString()).RemoveDialogAsync(dialog);
        }

        public async Task RemoveMessageAsync(Dialog dialog, Message message)
        {
            dialog.Messages = new List<Message> { message };
            dialog.MessageCount--;
            await _hubContext.Clients.User(dialog.Users[0].Id.ToString()).RemoveMessageAsync(dialog);
            await _hubContext.Clients.User(dialog.Users[1].Id.ToString()).RemoveMessageAsync(dialog);
        }

        public async Task UpdateMessageAsync(Dialog dialog, Message message, string text)
        {
            message.Text = text;
            dialog.Messages = new List<Message> { message };
            await _hubContext.Clients.User(dialog.Users[0].Id.ToString()).UpdateMessageAsync(dialog);
            await _hubContext.Clients.User(dialog.Users[1].Id.ToString()).UpdateMessageAsync(dialog);
        }
    }
}
