using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(DataContext context)
        {
            if (!context.Users.Any())
            {
                var user = new User
                {
                    Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
