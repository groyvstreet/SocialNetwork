using IdentityService.DAL.Interfaces;
using IdentityService.DAL.Repositories;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.BLL.AutoMapperProfiles;
using IdentityService.BLL.Validators.UserValidators;
using FluentValidation;
using FluentValidation.AspNetCore;

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

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, BLL.Services.IdentityService>();
        }
    }
}
