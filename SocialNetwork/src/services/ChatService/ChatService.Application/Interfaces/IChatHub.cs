using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces
{
    public interface IChatHub
    {
        Task SendToUser(string userId, Dialog dialog);

        Task UpdateMessage(string userId, Dialog dialog);

        Task RemoveMessage(string userId, Dialog dialog);

        Task RemoveDialog(string userId, Dialog dialog);
    }
}
