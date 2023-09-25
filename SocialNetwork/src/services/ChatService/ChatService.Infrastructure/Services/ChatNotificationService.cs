using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using ChatService.Infrastructure.Hubs.ChatHub;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Infrastructure.Services
{
    public class ChatNotificationService : IChatNotificationService
    {
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public ChatNotificationService(IHubContext<ChatHub, IChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task CreateChatAsync(Chat chat)
        {
            await _hubContext.Clients.User(chat.Users.First().Id.ToString()).CreateChatAsync(chat);
        }

        public async Task SendMessageAsync(Chat chat, Message message)
        {
            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users.Clear();
            chat.Messages = new List<Message> { message };
            await _hubContext.Clients.Users(userIds).SendMessageAsync(chat);
        }

        public async Task AddUsetToChatAsync(Chat chat, ChatUser user)
        {
            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users = new List<ChatUser> { user };
            chat.UserCount++;
            chat.Messages.Clear();
            await _hubContext.Clients.Users(userIds).AddUserToChatAsync(chat);
        }

        public async Task RemoveMessageAsync(Chat chat, Message message)
        {
            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users.Clear();
            chat.Messages = new List<Message> { message };
            chat.MessageCount--;
            await _hubContext.Clients.Users(userIds).RemoveMessageAsync(chat);
        }

        public async Task RemoveUserFromChatAsync(Chat chat, ChatUser user)
        {
            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users = new List<ChatUser> { user };
            chat.UserCount--;
            chat.Messages.Clear();
            await _hubContext.Clients.Users(userIds).RemoveUserFromChatAsync(chat);
        }

        public async Task SetUserAsChatAdminAsync(Chat chat, ChatUser user, bool isAdmin)
        {
            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            user.IsAdmin = isAdmin;
            chat.Users = new List<ChatUser> { user };
            chat.Messages.Clear();
            await _hubContext.Clients.Users(userIds).SetUserAsChatAdminAsync(chat);
        }

        public async Task UpdateChatAsync(Chat chat, UpdateChatDTO updateChatDTO)
        {
            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Name = updateChatDTO.Name;
            chat.Image = updateChatDTO.Image;
            chat.Messages.Clear();
            chat.Users.Clear();
            await _hubContext.Clients.Users(userIds).UpdateChatAsync(chat);
        }

        public async Task UpdateMessageAsync(Chat chat, Message message, string text)
        {
            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users.Clear();
            chat.Messages = new List<Message> { message };
            chat.Messages.First().Text = text;
            await _hubContext.Clients.Users(userIds).UpdateMessageAsync(chat);
        }
    }
}
