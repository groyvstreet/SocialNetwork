using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(IMongoDatabase mongoDatabase, string collectionName) : base(mongoDatabase, collectionName) { }

        public async Task AddChatMessageAsync(Guid chatId, Message message)
        {
            var pushMessage = _updateDefinitionBuilder.Push(c => c.Messages, message);
            await _collection.UpdateOneAsync(c => c.Id == chatId, pushMessage);

            var incMessageCount = _updateDefinitionBuilder.Inc(c => c.MessageCount, 1);
            await _collection.UpdateOneAsync(c => c.Id == chatId, incMessageCount);
        }

        public async Task AddUserToChatAsync(Guid chatId, ChatUser user)
        {
            var pushUser = _updateDefinitionBuilder.Push(c => c.Users, user);
            await _collection.UpdateOneAsync(c => c.Id == chatId, pushUser);

            var incUserCount = _updateDefinitionBuilder.Inc(c => c.UserCount, 1);
            await _collection.UpdateOneAsync(c => c.Id == chatId, incUserCount);
        }

        public async Task AddUserToInvitedUsers(Guid chatId, Guid userId, Guid invitedUserId)
        {
            var chat = await _collection.Find(c => c.Id == chatId).FirstOrDefaultAsync();
            var userIndex = chat.Users.FindIndex(u => u.Id == userId);
            var update = _updateDefinitionBuilder.AddToSet(c => c.Users[userIndex].InvitedUsers, invitedUserId.ToString());
            await _collection.UpdateOneAsync(c => c.Id == chatId, update);
        }

        public async Task<List<Chat>> GetChatsByUserIdAsync(Guid userId)
        {
            var chats = await _collection
                .Find(c => c.Users.Any(u => u.Id == userId))
                .Project(c => new Chat
                {
                    Id = c.Id,
                    Name = c.Name,
                    MessageCount = c.MessageCount,
                    Users = c.Users,
                    Messages = c.Messages.Where(m => m.UsersRemoved.All(s => s != userId.ToString())).ToList()
                }).ToListAsync();

            return chats;
        }

        public async Task RemoveChatMessageAsync(Guid chatId, Guid messageId)
        {
            var pullMessage = _updateDefinitionBuilder.PullFilter(d => d.Messages, m => m.Id == messageId);
            await _collection.UpdateOneAsync(c => c.Id == chatId, pullMessage);

            var decUserCount = _updateDefinitionBuilder.Inc(c => c.UserCount, -1);
            await _collection.UpdateOneAsync(c => c.Id == chatId, decUserCount);
        }

        public async Task RemoveChatMessageFromUserAsync(Guid chatId, Guid messageId, Guid userId)
        {
            var chat = await _collection.Find(c => c.Id == chatId).FirstOrDefaultAsync();
            var messageIndex = chat.Messages.FindIndex(m => m.Id == messageId);
            var update = _updateDefinitionBuilder.AddToSet(c => c.Messages[messageIndex].UsersRemoved, userId.ToString());
            await _collection.UpdateOneAsync(c => c.Id == chatId, update);
        }

        public async Task RemoveUserFromChatAsync(Guid chatId, Guid userId)
        {
            var pullUser = _updateDefinitionBuilder.PullFilter(c => c.Users, u => u.Id == userId);
            await _collection.UpdateOneAsync(c => c.Id == chatId, pullUser);

            var decUserCount = _updateDefinitionBuilder.Inc(c => c.UserCount, -1);
            await _collection.UpdateOneAsync(c => c.Id == chatId, decUserCount);
        }

        public async Task SetUserAsChatAdminAsync(Guid chatId, Guid userId, bool isAdmin)
        {
            var chat = await _collection.Find(c => c.Id == chatId).FirstOrDefaultAsync();
            var userIndex = chat.Users.FindIndex(u => u.Id == userId);
            var update = _updateDefinitionBuilder.Set(c => c.Users[userIndex].IsAdmin, isAdmin);
            await _collection.UpdateOneAsync(c => c.Id == chatId, update);
        }

        public async Task UpdateChatMessageAsync(Guid chatId, Guid messageId, string text)
        {
            var chat = await _collection.Find(c => c.Id == chatId).FirstOrDefaultAsync();
            var messageIndex = chat.Messages.FindIndex(m => m.Id == messageId);
            var update = _updateDefinitionBuilder.Set(d => d.Messages[messageIndex].Text, text);
            await _collection.UpdateOneAsync(c => c.Id == chatId, update);
        }
    }
}
