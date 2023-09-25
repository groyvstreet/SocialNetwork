using ChatService.Domain.Entities;

namespace ChatService.Infrastructure.Hubs.DialogHub
{
    public interface IDialogHub
    {
        Task SendMessageAsync(Dialog dialog);

        Task UpdateMessageAsync(Dialog dialog);

        Task RemoveMessageAsync(Dialog dialog);

        Task RemoveDialogAsync(Dialog dialog);
    }
}
