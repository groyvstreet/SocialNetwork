using IdentityService.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            return await roleManager.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string id)
        {
            return await roleManager.FindByIdAsync(id);
        }

        public async Task<IdentityRole?> GetRoleByNameAsync(string name)
        {
            return await roleManager.FindByNameAsync(name);
        }

        public async Task AddRoleAsync(IdentityRole role)
        {
            await roleManager.CreateAsync(role);
        }

        public async Task UpdateRoleAsync(IdentityRole role)
        {
            await roleManager.UpdateAsync(role);
        }

        public async Task RemoveRoleAsync(IdentityRole role)
        {
            await roleManager.DeleteAsync(role);
        }
    }
}
