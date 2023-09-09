using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.PL.Extensions
{
    public static class DatabaseExtensions
    {
        public static void AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var assemblyName = configuration.GetSection("MigrationsAssembly").Get<string>();
            services.AddDbContext<DataContext>(
                opt => opt.UseSqlServer(connectionString,
                                        sqlServerOpt => sqlServerOpt.MigrationsAssembly(assemblyName)));
        }

        public static void ApplyMigrations(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<DataContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        public static async Task InitializeDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            await DbInitializer.SeedData(roleManager);
            await DbInitializer.SeedData(userManager);
        }
    }
}
