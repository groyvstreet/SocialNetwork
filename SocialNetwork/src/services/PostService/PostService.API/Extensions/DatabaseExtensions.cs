using Microsoft.EntityFrameworkCore;
using PostService.Infrastructure.Data;

namespace PostService.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static void AddDatabaseConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var assemblyName = configuration.GetSection("MigrationsAssembly").Get<string>();
            services.AddDbContext<DataContext>(
                opt => opt.UseNpgsql(connectionString,
                                     npgsqlOpt => npgsqlOpt.MigrationsAssembly(assemblyName)));
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

        public static async Task InitializeDatabaseAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<DataContext>();

            await DbInitializer.SeedDataAsync(context);
        }
    }
}
