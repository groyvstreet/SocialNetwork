using ChatService.Application.AutoMapperProfiles;
using ChatService.Application.Behaviours;
using ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Infrastructure.Hubs.ChatHub;
using ChatService.Infrastructure.Hubs.DialogHub;
using ChatService.Infrastructure.Repositories;
using ChatService.Infrastructure.Services;
using MediatR;
using MongoDB.Driver;
using FluentValidation;
using ChatService.Application.Validators.DialogCommandValidators;
using ChatService.Application;
using ChatService.Infrastructure.Interfaces;
using ChatService.Infrastructure;
using ChatService.Domain.Entities;
using ChatService.Infrastructure.CacheRepositories;

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
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddValidatorsFromAssemblyContaining<AddDialogMessageCommandValidator>();
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
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

            services.Configure<KafkaConsumerConfig<RequestOperation, User>>(ko =>
            {
                var section = configuration.GetSection("KafkaOptions");
                ko.BootstrapServers = section.GetSection("BootstrapServers").Get<string>();
                ko.GroupId = section.GetSection("GroupId").Get<string>();
                ko.Topic = "users";
            });
            services.AddHostedService<KafkaConsumerService<RequestOperation, User>>();
            services.AddTransient<IKafkaConsumerHandler<RequestOperation, User>, UserKafkaConsumerHandler>();
        }

        public static void MapSignalR(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<DialogHub>("/dialogs");
            endpointRouteBuilder.MapHub<ChatHub>("/chats");
        }

        public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(redisCacheOptions =>
            {
                redisCacheOptions.Configuration = configuration.GetSection("RedisConnection").Get<string>();
            });
            
            services.AddScoped<ICacheRepository<User>, CacheRepository<User>>();
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
