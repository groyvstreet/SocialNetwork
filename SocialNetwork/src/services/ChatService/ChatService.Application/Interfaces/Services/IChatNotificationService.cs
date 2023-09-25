using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces.Services
{
    public interface IChatNotificationService
    {
        Task CreateChatAsync(Chat chat);

        Task SendMessageAsync(Chat chat, Message message);

        Task AddUsetToChatAsync(Chat chat, ChatUser user);

        Task RemoveMessageAsync(Chat chat, Message message);

        Task RemoveUserFromChatAsync(Chat chat, ChatUser user);

        Task SetUserAsChatAdminAsync(Chat chat, ChatUser user, bool isAdmin);

        Task UpdateChatAsync(Chat chat, UpdateChatDTO updateChatDTO);

        Task UpdateMessageAsync(Chat chat, Message message, string text);
    }
}
