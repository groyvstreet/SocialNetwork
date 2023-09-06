using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityService.BLL.Interfaces
{
    public interface IIdentityService
    {
        Task<GetUserDTO> SignUp(AddUserDTO addUserDTO);

        Task<string> SignInByJwt(string email, string password);
    }
}
