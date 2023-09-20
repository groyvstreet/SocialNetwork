using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<IdentityRole>> GetRolesAsync();

        Task<IdentityRole?> GetRoleByIdAsync(string id);

        Task<IdentityRole?> GetRoleByNameAsync(string name);

        Task AddRoleAsync(IdentityRole role);

        Task UpdateRoleAsync(IdentityRole role);

        Task RemoveRoleAsync(IdentityRole role);
    }
}
