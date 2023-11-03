using FluentValidation;
using FluentValidation.AspNetCore;
using PostService.Application;
using PostService.Application.AutoMapperProfiles;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Application.Validators.PostValidators;
using PostService.Domain.Entities;
using PostService.Infrastructure;
using PostService.Infrastructure.CacheRepositories;
using PostService.Infrastructure.Interfaces;
using PostService.Infrastructure.Repositories;
using PostService.Infrastructure.Services;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace PostService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AddPostValidator>();
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile).Assembly);
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostLikeRepository, PostLikeRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, Application.Services.PostService>();
            services.AddScoped<IPostLikeService, PostLikeService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ICommentLikeService, CommentLikeService>();
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

        public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(redisCacheOptions =>
            {
                redisCacheOptions.Configuration = configuration.GetConnectionString("Redis");
            });

            services.AddScoped<ICacheRepository<User>, CacheRepository<User>>();
            services.AddScoped<ICacheRepository<Post>, CacheRepository<Post>>();
            services.AddScoped<ICacheRepository<Comment>, CacheRepository<Comment>>();
            services.AddScoped<ICacheRepository<PostLike>, CacheRepository<PostLike>>();
            services.AddScoped<ICacheRepository<CommentLike>, CacheRepository<CommentLike>>();
        }
      
        public static void MapGrpc(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapGrpcService<Application.Grpc.Services.PostService>();
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
    }
}
