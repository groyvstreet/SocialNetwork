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
                    Id = Guid.Parse("ec52651d-0237-45a9-bd56-1ac765b9457b"),
                    FirstName = "Bam",
                    LastName = "User",
                    Image = "https://plus.unsplash.com/premium_photo-1664474619075-644dd191935f?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=869&q=80"
                },
                new User
                {
                    Id = Guid.Parse("6ed9b92b-488e-4e23-a496-14a3de16ce81"),
                    FirstName = "Lyaps",
                    LastName = "User",
                    Image = "https://www.simplilearn.com/ice9/free_resources_article_thumb/what_is_image_Processing.jpg"
                },
                new User
                {
                    Id = Guid.Parse("aed9b92b-488e-4e23-a496-14a3de16ce84"),
                    FirstName = "Ai",
                    LastName = "User",
                    Image = "https://www.simplilearn.com/ice9/free_resources_article_thumb/what_is_image_Processing.jpg"
                }
            };
            await usersCollection.InsertManyAsync(users);
        }
    }
}
