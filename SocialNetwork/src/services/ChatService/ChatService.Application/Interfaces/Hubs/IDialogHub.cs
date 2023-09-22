using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces.Hubs
{
    public interface IDialogHub
    {
        Task SendMessage(Dialog dialog);

        Task UpdateMessage(Dialog dialog);

        Task RemoveMessage(Dialog dialog);

        Task RemoveDialog(Dialog dialog);
    }
}
