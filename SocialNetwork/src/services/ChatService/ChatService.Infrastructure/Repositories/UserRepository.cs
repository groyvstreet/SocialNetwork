using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoDatabase mongoDatabase, string collectionName) : base(mongoDatabase, collectionName) { }

        public async Task UpdateAsync(User user)
        {
            var update = _updateDefinitionBuilder.Set(u => u.FirstName, user.FirstName)
                .Set(u => u.LastName, user.LastName)
                .Set(u => u.Image, user.Image);
            await _collection.UpdateOneAsync(u => u.Id == user.Id, update);
        }
    }
}
