using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> userManager;

        public UserRepository(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await userManager.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return null;
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
            
            if (!isPasswordValid)
            {
                return null;
            }

            return user;
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            var roles = await userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task AddUserAsync(User user, string password)
        {
            await userManager.CreateAsync(user, password);
        }

        public async Task UpdateUserAsync(User user)
        {
            await userManager.UpdateAsync(user);
        }

        public async Task RemoveUserAsync(User user)
        {
            await userManager.DeleteAsync(user);
        }
    }
}
