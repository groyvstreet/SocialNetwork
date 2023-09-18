using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ChatService.Infrastructure.Repositories
{
    public class DialogRepository : BaseRepository<Dialog>, IDialogRepository
    {
        public DialogRepository(IMongoDatabase mongoDatabase, string collectionName) : base(mongoDatabase, collectionName) { }

        public async Task<Dialog?> GetDialogWithSingleMessage(Guid dialogId, Guid messageId)
        {
            var dialog = await _collection.Find(c => c.Id == dialogId).Project(d => new Dialog
            {
                Id = d.Id,
                MessageCount = d.MessageCount,
                Messages = d.Messages.Where(m => m.Id == messageId).ToList()
            }).FirstOrDefaultAsync();

            return dialog;
        }

        public async Task<List<Dialog>> GetDialogsByUserIdAsync(Guid userId)
        {
            var aggregation = _collection.Aggregate()
                .Match(d => d.Users.Any(u => u.Id == userId))
                .Project(d => new Dialog
                {
                    Id = d.Id,
                    MessageCount = d.MessageCount,
                    Users = d.Users,
                    Messages = d.Messages.Where(m => m.UsersRemoved.All(s => s != userId.ToString())).ToList()
                });

            return await aggregation.ToListAsync();
        }

        public async Task AddDialogMessageAsync(Guid dialogId, Message message)
        {
            var update = _updateDefinitionBuilder.Push(d => d.Messages, message);
            await _collection.UpdateOneAsync(d => d.Id == dialogId, update);
        }

        public async Task UpdateDialogMessageAsync(Guid dialogId, Guid messageId, string text)
        {
            var dialog = await _collection.Find(d => d.Id == dialogId).FirstOrDefaultAsync();
            var messageIndex = dialog.Messages.FindIndex(m => m.Id == messageId);
            var update = _updateDefinitionBuilder.Set(d => d.Messages[messageIndex].Text, text);
            await _collection.UpdateOneAsync(d => d.Id == dialogId, update);
        }

        public async Task RemoveDialogMessageAsync(Guid dialogId, Guid messageId)
        {
            var update = _updateDefinitionBuilder.PullFilter(d => d.Messages, m => m.Id == messageId);
            await _collection.UpdateOneAsync(d => d.Id == dialogId, update);
        }

        public async Task RemoveDialogMessageFromUserAsync(Guid dialogId, Guid messageId, Guid userId)
        {
            var dialog = await _collection.Find(d => d.Id == dialogId).FirstOrDefaultAsync();
            var messageIndex = dialog.Messages.FindIndex(m => m.Id == messageId);
            var update = _updateDefinitionBuilder.AddToSet(d => d.Messages[messageIndex].UsersRemoved, userId.ToString());
            await _collection.UpdateOneAsync(d => d.Id == dialogId, update);
        }
    }
}
