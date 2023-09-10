using IdentityService.DAL.Interfaces;
using IdentityService.DAL.Repositories;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;

namespace IdentityService.PL.Extensions
{
    public static class ServiceExtensions
    {
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
