using IdentityService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            return await _roleManager.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<IdentityRole?> GetRoleByNameAsync(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }

        public async Task AddRoleAsync(IdentityRole role)
        {
            await _roleManager.CreateAsync(role);
        }

        public async Task UpdateRoleAsync(IdentityRole role)
        {
            await _roleManager.UpdateAsync(role);
        }

        public async Task RemoveRoleAsync(IdentityRole role)
        {
            await _roleManager.DeleteAsync(role);
        }
    }
}
