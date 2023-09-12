using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using ChatService.Infrastructure.Repositories;
using MongoDB.Driver;

namespace ChatService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {
            services.AddScoped<IBaseRepository<T>>(provider =>
            {
                var mongoDatabase = provider.GetService<IMongoDatabase>()!;

                return new BaseRepository<T>(mongoDatabase, collectionName);
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddRepository<Dialog>("dialogs");
        }
    }
}
