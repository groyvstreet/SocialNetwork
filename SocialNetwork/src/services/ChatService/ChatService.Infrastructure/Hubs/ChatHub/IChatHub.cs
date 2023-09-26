using ChatService.Domain.Entities;

namespace ChatService.Infrastructure.Hubs.ChatHub
{
    public interface IChatHub
    {
        Task CreateChatAsync(Chat chat);

        Task UpdateChatAsync(Chat chat);

        Task AddUserToChatAsync(Chat chat);

        Task RemoveUserFromChatAsync(Chat chat);

        Task SetUserAsChatAdminAsync(Chat chat);

        Task SetUserAsDefaultAsync(Chat chat);

        Task SendMessageAsync(Chat chat);

        Task UpdateMessageAsync(Chat chat);

        Task RemoveMessageAsync(Chat chat);
    }
}
