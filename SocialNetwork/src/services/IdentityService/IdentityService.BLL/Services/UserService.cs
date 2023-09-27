using AutoMapper;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Data;
using IdentityService.DAL.Interfaces;
using System.Text.Json;

namespace IdentityService.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducerService _kafkaProducerService;

        public UserService(IMapper mapper,
                           IUserRepository userRepository,
                           IKafkaProducerService kafkaProducerService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<List<GetUserDTO>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            var getUserDTOs = users.Select(_mapper.Map<GetUserDTO>).ToList();

            return getUserDTOs;
        }

        public async Task<GetUserDTO> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {id}");
            }

            var getUserDTO = _mapper.Map<GetUserDTO>(user);

            return getUserDTO;
        }

        public async Task<GetUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDTO, string authenticatedUserId, string authenticatedUserRole)
        {
            if (authenticatedUserRole != Roles.Admin && authenticatedUserId != updateUserDTO.Id)
            {
                throw new ForbiddenException();
            }

            var user = await _userRepository.GetUserByIdAsync(updateUserDTO.Id);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {updateUserDTO.Id}");
            }

            user.FirstName = updateUserDTO.FirstName;
            user.LastName = updateUserDTO.LastName;
            user.BirthDate = updateUserDTO.BirthDate;
            await _userRepository.UpdateUserAsync(user);
            var getUserDTO = _mapper.Map<GetUserDTO>(user);

            await _kafkaProducerService.SendUserRequestAsync(UserRequest.Update, getUserDTO);

            return getUserDTO;
        }

        public async Task RemoveUserByIdAsync(string id, string authenticatedUserId, string authenticatedUserRole)
        {
            if (authenticatedUserRole != Roles.Admin && authenticatedUserId != id)
            {
                throw new ForbiddenException();
            }

            var user = await _userRepository.GetUserByIdAsync(id);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {id}");
            }

            await _userRepository.RemoveUserAsync(user);

            var getUserDTO = _mapper.Map<GetUserDTO>(user);
            await _kafkaProducerService.SendUserRequestAsync(UserRequest.Remove, getUserDTO);
        }
    }
}
