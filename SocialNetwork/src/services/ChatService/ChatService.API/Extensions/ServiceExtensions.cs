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
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Mongo.Migration.Strategies;
using ChatService.API.Hangfire;
using ChatService.Application.Interfaces.Services.Hangfire;
using ChatService.Infrastructure.Services.Hangfire;
using ChatService.Application.Services.Hangfire;
using ChatService.Application;
using ChatService.Infrastructure.Interfaces;
using ChatService.Infrastructure;
using ChatService.Domain.Entities;
using ChatService.Infrastructure.CacheRepositories;
using ChatService.Application.Grpc.Protos;
using ChatService.Application.Grpc.Services;

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

        public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetSection("MongoConnection").Get<string>();
            var database = configuration.GetSection("MongoDatabase").Get<string>();

            var mongoMigrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new DropMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            };

            var mongoStorageOptions = new MongoStorageOptions
            {
                MigrationOptions = mongoMigrationOptions,
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                CheckConnection = false
            };

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                config.UseSimpleAssemblyNameTypeSerializer();
                config.UseRecommendedSerializerSettings();
                config.UseMongoStorage(connection, database, mongoStorageOptions);
            });
            services.AddHangfireServer();
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
            services.AddScoped<IBackgroundJobService, BackgroundJobService>();
            services.AddScoped<IChatService, Application.Services.ChatService>();
            services.AddHostedService<RecurringJobExecutorService>();
        }

        public static void AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaConsumerConfig<RequestOperation, User>>(kafkaConsumerConfig =>
            {
                var section = configuration.GetSection("KafkaOptions");
                kafkaConsumerConfig.BootstrapServers = section.GetSection("BootstrapServers").Get<string>();
                kafkaConsumerConfig.GroupId = section.GetSection("GroupId").Get<string>();
                kafkaConsumerConfig.Topic = "users";
            });

            services.AddHostedService<KafkaConsumerService<RequestOperation, User>>();
            services.AddTransient<IKafkaConsumerHandler<RequestOperation, User>, UserKafkaConsumerHandler>();
        }

        public static void AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<Post.PostClient>(grpcClientFactoryOptions =>
            {
                var address = configuration.GetSection("GrpcOptions").GetSection("Address").Get<string>();
                grpcClientFactoryOptions.Address = new Uri(address ?? string.Empty);
            });

            services.AddScoped<IPostService, PostService>();
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

        public static void UseHangfireDashboardUI(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });
        }
    }
}
