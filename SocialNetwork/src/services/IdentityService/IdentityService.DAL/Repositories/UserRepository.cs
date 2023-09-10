using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _userManager.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return null;
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            
            if (!isPasswordValid)
            {
                return null;
            }

            return user;
        }

        public async Task<List<string>> GetUserRolesAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task AddUserAsync(User user, string password, string role)
        {
            await _userManager.CreateAsync(user, password);
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task RemoveUserAsync(User user)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}
