using AutoMapper;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;

namespace IdentityService.BLL.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public IdentityService(IMapper mapper,
                               IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        public async Task<GetUserDTO> SignUp(AddUserDTO addUserDTO)
        {
            var user = await userRepository.GetUserByEmailAsync(addUserDTO.Email);

            if (user is not null)
            {
                throw new AlreadyExistsException($"user with email = {addUserDTO.Email} already exists");
            }

            user = mapper.Map<User>(addUserDTO);
            user.UserName = user.Email;
            await userRepository.AddUserAsync(user, addUserDTO.Password);
            var getUserDTO = mapper.Map<GetUserDTO>(user);

            return getUserDTO;
        }
    }
}
