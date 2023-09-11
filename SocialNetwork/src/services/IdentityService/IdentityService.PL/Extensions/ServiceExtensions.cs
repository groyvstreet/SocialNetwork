using IdentityService.DAL.Interfaces;
using IdentityService.DAL.Repositories;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.BLL.AutoMapperProfiles;
using IdentityService.BLL.Validators.UserValidators;
using FluentValidation;
using IdentityService.BLL.DTOs.UserDTOs;
using FluentValidation.AspNetCore;

namespace IdentityService.PL.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AddUserDTO>, AddUserValidator>();
            services.AddScoped<IValidator<UpdateUserDTO>, UpdateUserValidator>();

            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile));
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IIdentityService, BLL.Services.IdentityService>();
        }
    }
}
