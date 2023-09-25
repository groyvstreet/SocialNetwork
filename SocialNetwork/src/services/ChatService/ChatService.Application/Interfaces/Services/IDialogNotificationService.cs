using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces.Services
{
    public interface IDialogNotificationService
    {
        Task SendMessageAsync(Dialog dialog, Message message);

        Task UpdateMessageAsync(Dialog dialog, Message message, string text);

        Task RemoveMessageAsync(Dialog dialog, Message message);

        Task RemoveDialogAsync(Dialog dialog);
    }
}
