using AutoMapper;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Data;
using IdentityService.DAL.Interfaces;

namespace IdentityService.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public UserService(IMapper mapper,
                           IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        public async Task<List<GetUserDTO>> GetUsersAsync()
        {
            var users = await userRepository.GetUsersAsync();
            var getUserDTOs = users.Select(mapper.Map<GetUserDTO>).ToList();

            return getUserDTOs;
        }

        public async Task<GetUserDTO> GetUserByIdAsync(string id)
        {
            var user = await userRepository.GetUserByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {id}");
            }

            var getUserDTO = mapper.Map<GetUserDTO>(user);

            return getUserDTO;
        }

        public async Task<GetUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDTO, string authenticatedUserId, string authenticatedUserRole)
        {
            if (authenticatedUserRole != Roles.Admin && authenticatedUserId != updateUserDTO.Id)
            {
                throw new ForbiddenException();
            }

            var user = await userRepository.GetUserByIdAsync(updateUserDTO.Id);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {updateUserDTO.Id}");
            }

            user.FirstName = updateUserDTO.FirstName;
            user.LastName = updateUserDTO.LastName;
            user.BirthDate = updateUserDTO.BirthDate;
            await userRepository.UpdateUserAsync(user);
            var getUserDTO = mapper.Map<GetUserDTO>(user);

            return getUserDTO;
        }

        public async Task RemoveUserByIdAsync(string id, string authenticatedUserId, string authenticatedUserRole)
        {
            if (authenticatedUserRole != Roles.Admin && authenticatedUserId != id)
            {
                throw new ForbiddenException();
            }

            var user = await userRepository.GetUserByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {id}");
            }

            await userRepository.RemoveUserAsync(user);
        }
    }
}
