using IdentityService.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
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
