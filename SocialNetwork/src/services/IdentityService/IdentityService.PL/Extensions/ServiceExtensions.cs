using IdentityService.DAL.Interfaces;
using IdentityService.DAL.Repositories;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.BLL.AutoMapperProfiles;
using IdentityService.BLL.Validators.UserValidators;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityService.BLL;
using IdentityService.BLL.DTOs.UserDTOs;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using System.Reflection;
using IdentityService.DAL.CacheRepositories;
using IdentityService.DAL.Entities;

namespace IdentityService.PL.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AddUserValidator>();
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile));
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, BLL.Services.IdentityService>();

            services.Configure<KafkaProducerConfig<RequestOperation, GetUserDTO>>(ko =>
            {
                var section = configuration.GetSection("KafkaOptions");
                ko.BootstrapServers = section.GetSection("BootstrapServers").Get<string>();
                ko.Topic = "users";
            });
            services.AddSingleton<IKafkaProducerService<RequestOperation, GetUserDTO>, KafkaProducerService<RequestOperation, GetUserDTO>>();
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
        
        public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(redisCacheOptions =>
            {
                redisCacheOptions.Configuration = configuration.GetConnectionString("Redis");
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
