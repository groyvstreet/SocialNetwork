using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces.Repositories
{
    public interface IChatRepository : IBaseRepository<Chat>
    {
        Task<List<Chat>> GetChatsByUserIdAsync(Guid userId);

        Task AddChatMessageAsync(Guid chatId, Message message);

        Task UpdateChatMessageAsync(Guid chatId, Guid messageId, string text);

        Task RemoveChatMessageAsync(Guid chatId, Guid messageId);

        Task RemoveChatMessageFromUserAsync(Guid chatId, Guid messageId, Guid userId);

        Task AddUserToChatAsync(Guid chatId, ChatUser user);

        Task AddUserToInvitedUsers(Guid chatId, Guid userId, Guid invitedUserId);

        Task RemoveUserFromChatAsync(Guid chatId, Guid userId);

        Task SetUserAsChatAdminAsync(Guid chatId, Guid userId, bool isAdmin);

        Task UpdateUserAsync(User user);

        Task RemoveUserAsync(User user);
    }
}
