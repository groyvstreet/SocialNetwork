﻿using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces
{
    public interface IDialogRepository : IBaseRepository<Dialog>
    {
        Task<List<Dialog>> GetDialogsByUserIdAsync(Guid userId);

        Task AddDialogMessageAsync(Guid dialogId, Message message);

        Task UpdateDialogMessageAsync(Guid dialogId, Guid messageId, string text);

        Task RemoveDialogMessageAsync(Guid dialogId, Guid messageId);

        Task RemoveDialogMessageFromUserAsync(Guid dialogId, Guid messageId, Guid userId);
    }
}
