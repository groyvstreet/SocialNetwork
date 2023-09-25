using ChatService.Application.AutoMapperProfiles;
using ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Infrastructure.Hubs.ChatHub;
using ChatService.Infrastructure.Hubs.DialogHub;
using ChatService.Infrastructure.Repositories;
using ChatService.Infrastructure.Services;
using MediatR;
using MongoDB.Driver;

namespace ChatService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DialogProfile).Assembly);
        }

        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(typeof(AddDialogMessageCommandHandler).Assembly);
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository>(provider =>
            {
                var mongoDatabase = provider.GetService<IMongoDatabase>()!;

                return new UserRepository(mongoDatabase, "users");
            });
            services.AddScoped<IDialogRepository>(provider =>
            {
                var mongoDatabase = provider.GetService<IMongoDatabase>()!;

                return new DialogRepository(mongoDatabase, "dialogs");
            });
            services.AddScoped<IChatRepository>(provider =>
            {
                var mongoDatabase = provider.GetService<IMongoDatabase>()!;

                return new ChatRepository(mongoDatabase, "chats");
            });
            services.AddScoped<IDialogNotificationService, DialogNotificationService>();
            services.AddScoped<IChatNotificationService, ChatNotificationService>();
        }

        public static void MapSignalR(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<DialogHub>("/dialogs");
            endpointRouteBuilder.MapHub<ChatHub>("/chats");
        }

        public static void AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://127.0.0.1:3000")
                        .AllowCredentials()
                        .SetIsOriginAllowed((hosts) => true));
            });
        }
    }
}
