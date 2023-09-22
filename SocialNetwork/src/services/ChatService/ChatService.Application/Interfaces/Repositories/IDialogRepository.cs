using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces.Repositories
{
    public interface IDialogRepository : IBaseRepository<Dialog>
    {
        Task<List<Dialog>> GetDialogsByUserIdAsync(Guid userId);

        Task<Dialog?> GetDialogWithSingleMessage(Guid dialogId, Guid messageId);

        Task AddDialogMessageAsync(Guid dialogId, Message message);

        Task UpdateDialogMessageAsync(Guid dialogId, Guid messageId, string text);

        Task RemoveDialogMessageAsync(Guid dialogId, Guid messageId);

        Task RemoveDialogMessageFromUserAsync(Guid dialogId, Guid messageId, Guid userId);
    }
}
