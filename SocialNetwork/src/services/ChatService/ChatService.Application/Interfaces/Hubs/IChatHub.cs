using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces.Hubs
{
    public interface IChatHub
    {
        Task CreateChat(Chat chat);

        Task UpdateChat(Chat chat);

        Task AddUserToChat(Chat chat);

        Task RemoveUserFromChat(Chat chat);

        Task SetUserAsChatAdmin(Chat chat);

        Task SetUserAsDefault(Chat chat);

        Task SendMessage(Chat chat);

        Task UpdateMessage(Chat chat);

        Task RemoveMessage(Chat chat);
    }
}
