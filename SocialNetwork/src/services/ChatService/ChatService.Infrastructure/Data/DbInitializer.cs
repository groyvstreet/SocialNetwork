using ChatService.Domain.Entities;
using MongoDB.Driver;

namespace ChatService.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(IMongoDatabase mongoDatabase)
        {
            var usersCollection = mongoDatabase.GetCollection<User>("users");
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "BAM",
                    LastName = "USER"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "LYAPS",
                    LastName = "USER"
                }
            };
            await usersCollection.InsertManyAsync(users);
        }
    }
}
