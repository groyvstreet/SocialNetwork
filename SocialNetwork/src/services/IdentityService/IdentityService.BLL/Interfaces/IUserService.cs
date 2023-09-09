using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityService.BLL.Interfaces
{
    public interface IUserService
    {
        Task<List<GetUserDTO>> GetUsersAsync();

        Task<GetUserDTO> GetUserByIdAsync(string id);

        Task<GetUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDTO, string authenticatedUserId, string authenticatedUserRole);

        Task RemoveUserByIdAsync(string id, string authenticatedUserId, string authenticatedUserRole);
    }
}
