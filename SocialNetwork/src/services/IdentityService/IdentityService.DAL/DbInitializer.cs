using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL
{
    public static class DbInitializer
    {
        public static async Task SeedData(RoleManager<IdentityRole> roleManager, List<string> roles)
        {
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        public static async Task SeedData(UserManager<User> userManager)
        {
            var user = new User()
            {
                Email = "admin",
                UserName = "admin",
                FirstName = "admin",
                LastName = "admin",
            };
            await userManager.CreateAsync(user, "string");
            await userManager.AddToRoleAsync(user, "admin");
        }
    }
}
