using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories
{
    public class DialogRepository : BaseRepository<Dialog>, IDialogRepository
    {
        private readonly IMongoCollection<Dialog> _collection;
        private readonly FilterDefinitionBuilder<Dialog> _filterBuilder;

        public DialogRepository(IMongoDatabase mongoDatabase, string collectionName) : base(mongoDatabase, collectionName)
        {
            _collection = mongoDatabase.GetCollection<Dialog>(collectionName);
            _filterBuilder = Builders<Dialog>.Filter;
        }

        public async Task<Dialog?> GetDialogByUsers(Guid senderId, Guid receiverId)
        {
            if (_collection.CountDocuments(_filterBuilder.Empty) == 0)
            {
                return null;
            }

            var filter = new BsonDocument("users",
                new BsonDocument
                {
                    {
                        "$elemMatch",
                        new BsonDocument
                        {
                            {
                                "$or",
                                new BsonArray
                                {
                                    new BsonDocument("Id", new BsonDocument("$eq", senderId.ToString())),
                                    new BsonDocument("Id", new BsonDocument("$eq", receiverId.ToString()))
                                }
                            }
                        }
                    }
                });

            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddMessageToDialogAsync(Guid dialogId, Message message)
        {
            var updateSettings = new BsonDocument("$push", new BsonDocument("Messages", message.ToBsonDocument()));
            await _collection.UpdateOneAsync(_filterBuilder.Eq(d => d.Id, dialogId), updateSettings);
        }
    }
}
