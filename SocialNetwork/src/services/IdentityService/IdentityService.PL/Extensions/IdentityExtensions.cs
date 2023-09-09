using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using IdentityService.DAL.Data;

namespace IdentityService.PL.Extensions
{
    public static class IdentityExtensions
    {
        public static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options => {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
        }
    }
}
