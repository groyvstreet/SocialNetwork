using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace ChatService.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : IEntity
    {
        protected readonly IMongoCollection<T> _collection;
        protected readonly FilterDefinitionBuilder<T> _filterDefinitionBuilder;
        protected readonly UpdateDefinitionBuilder<T> _updateDefinitionBuilder;
        protected readonly ProjectionDefinitionBuilder<T> _projectionDefinitionBuilder;

        public BaseRepository(IMongoDatabase mongoDatabase, string collectionName)
        {
            _collection = mongoDatabase.GetCollection<T>(collectionName);
            _filterDefinitionBuilder = Builders<T>.Filter;
            _updateDefinitionBuilder = Builders<T>.Update;
            _projectionDefinitionBuilder = Builders<T>.Projection;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_filterDefinitionBuilder.Empty).ToListAsync();
        }

        public async Task<List<T>> GetAllByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateFieldAsync<K>(T entity, Expression<Func<T, K>> field, K value)
        {
            var update = _updateDefinitionBuilder.Set(field, value);
            await _collection.UpdateOneAsync(e => e.Id == entity.Id, update);
        }

        public async Task RemoveAsync(T entity)
        {
            await _collection.DeleteOneAsync(e => e.Id == entity.Id);
        }
    }
}
