using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoDatabase mongoDatabase, string collectionName) : base(mongoDatabase, collectionName) { }
    }
}
