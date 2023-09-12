using MongoDB.Driver;

namespace ChatService.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static void AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetSection("MongoConnection").Get<string>();
            var database = configuration.GetSection("MongoDatabase").Get<string>();
            services.AddSingleton(new MongoClient(connection).GetDatabase(database));
        }
    }
}
