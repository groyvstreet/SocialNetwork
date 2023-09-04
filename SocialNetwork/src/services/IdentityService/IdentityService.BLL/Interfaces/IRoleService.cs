using IdentityService.BLL.DTOs.RoleDTOs;

namespace IdentityService.BLL.Interfaces
{
    public interface IRoleService
    {
        Task<List<GetRoleDTO>> GetRolesAsync();

        Task<GetRoleDTO>  GetRoleByIdAsync(string id);

        Task<GetRoleDTO>  GetRoleByNameAsync(string name);

        Task<GetRoleDTO> AddRoleAsync(AddRoleDTO addRoleDTO);

        Task<GetRoleDTO> UpdateRoleAsync(UpdateRoleDTO updateRoleDTO);

        Task RemoveRoleByIdAsync(string id);
    }
}
