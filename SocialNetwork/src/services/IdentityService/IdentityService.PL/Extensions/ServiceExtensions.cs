using IdentityService.DAL.Interfaces;
using IdentityService.DAL.Repositories;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.BLL.AutoMapperProfiles;
using IdentityService.BLL.Validators.UserValidators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using IdentityService.BLL;
using IdentityService.DAL.Entities;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.DAL.CacheRepositories;

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
