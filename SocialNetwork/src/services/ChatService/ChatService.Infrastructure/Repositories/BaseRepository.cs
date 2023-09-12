using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _collection;
        private readonly FilterDefinitionBuilder<T> _filterBuilder;

        public BaseRepository(IMongoDatabase mongoDatabase, string collectionName)
        {
            _collection = mongoDatabase.GetCollection<T>(collectionName);
            _filterBuilder = Builders<T>.Filter;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_filterBuilder.Empty).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultByIdAsync(Guid id)
        {
            return await _collection.Find(_filterBuilder.Eq(e => e.Id, id)).FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            await _collection.ReplaceOneAsync(_filterBuilder.Eq(e => e.Id, entity.Id), entity);
        }

        public async Task RemoveByIdAsync(Guid id)
        {
            await _collection.DeleteOneAsync(_filterBuilder.Eq(e => e.Id, id));
        }
    }
}
