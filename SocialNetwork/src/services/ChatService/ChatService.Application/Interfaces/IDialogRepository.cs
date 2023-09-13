using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces
{
    public interface IDialogRepository : IBaseRepository<Dialog>
    {
        Task<Dialog?> GetDialogByUsers(Guid senderId, Guid receiverId);

        Task AddMessageToDialogAsync(Guid dialogId, Message message);
    }
}
