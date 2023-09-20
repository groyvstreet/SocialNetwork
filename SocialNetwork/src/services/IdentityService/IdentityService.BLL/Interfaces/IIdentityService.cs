using IdentityService.BLL.DTOs.IdentityDTOs;
using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityService.BLL.Interfaces
{
    public interface IIdentityService
    {
        Task<GetUserDTO> SignUpAsync(AddUserDTO addUserDTO);

        Task<AuthenticatedResponseDTO> SignInAsync(string email, string password);
    }
}
