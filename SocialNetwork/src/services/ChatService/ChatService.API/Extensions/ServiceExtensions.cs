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
using ChatService.Application.Grpc.Protos;
using ChatService.Application.Grpc.Services;
using System.Reflection;
using Serilog.Sinks.Elasticsearch;
using Serilog;

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

        public static void ConfigureLogging(this IConfiguration configuration)
        {
            var stringUri = configuration.GetSection("ElasticConfiguration").GetSection("Uri").Get<string>()!;
            var node = new Uri(stringUri);
            var environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var options = new ElasticsearchSinkOptions(node)
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly()
                    .GetName().Name
                    ?.ToLower()
                    .Replace('.', '-')}-{environmentVariable?.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
                NumberOfReplicas = 1,
                NumberOfShards = 2
            };

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Elasticsearch(options)
                .Enrich.WithProperty("Environment", environmentVariable)
                .CreateLogger();
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
