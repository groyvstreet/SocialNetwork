using AutoMapper;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IdentityService.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IKafkaProducerService<RequestOperation, GetUserDTO> _kafkaProducerService;
        private readonly ILogger<UserService> _logger;
        private readonly ICacheRepository<User> _userCacheRepository;

        public UserService(IMapper mapper,
                           IUserRepository userRepository,
                           IKafkaProducerService<RequestOperation, GetUserDTO> kafkaProducerService,
                           ILogger<UserService> logger,
                           ICacheRepository<User> userCacheRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _kafkaProducerService = kafkaProducerService;
            _logger = logger;
            _userCacheRepository = userCacheRepository;
        }

        public async Task<List<GetUserDTO>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            var getUserDTOs = users.Select(_mapper.Map<GetUserDTO>).ToList();

            _logger.LogInformation("users - {users} getted", JsonSerializer.Serialize(getUserDTOs));

            return getUserDTOs;
        }

        public async Task<GetUserDTO> GetUserByIdAsync(string id)
        {
            var user = await _userCacheRepository.GetAsync(id);

            if (user is null)
            {
                user = await _userRepository.GetUserByIdAsync(id);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {id}");
                }

                await _userCacheRepository.SetAsync(user.Id, user);
            }

            var getUserDTO = _mapper.Map<GetUserDTO>(user);

            _logger.LogInformation("user - {user} getted", JsonSerializer.Serialize(getUserDTO));

            return getUserDTO;
        }

        public async Task<GetUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDTO,
            string authenticatedUserId,
            string authenticatedUserRole)
        {
            if (authenticatedUserRole != Roles.Admin && authenticatedUserId != updateUserDTO.Id)
            {
                throw new ForbiddenException();
            }

            var user = await _userCacheRepository.GetAsync(updateUserDTO.Id);

            if (user is null)
            {
                user = await _userRepository.GetUserByIdAsync(updateUserDTO.Id);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {updateUserDTO.Id}");
                }
            }

            user.FirstName = updateUserDTO.FirstName;
            user.LastName = updateUserDTO.LastName;
            user.BirthDate = updateUserDTO.BirthDate.ToDateTime(TimeOnly.MinValue);
            user.Image = updateUserDTO.Image;
            await _userRepository.UpdateUserAsync(user);
            var getUserDTO = _mapper.Map<GetUserDTO>(user);

            await _userCacheRepository.SetAsync(user.Id, user);

            await _kafkaProducerService.SendUserRequestAsync(RequestOperation.Update, getUserDTO);

            _logger.LogInformation("user - {user} updated", getUserDTO);

            return getUserDTO;
        }

        public async Task RemoveUserByIdAsync(string id, string authenticatedUserId, string authenticatedUserRole)
        {
            if (authenticatedUserRole != Roles.Admin && authenticatedUserId != id)
            {
                throw new ForbiddenException();
            }

            var user = await _userCacheRepository.GetAsync(id);

            if (user is null)
            {
                user = await _userRepository.GetUserByIdAsync(id);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {id}");
                }
            }

            await _userRepository.RemoveUserAsync(user);

            await _userCacheRepository.RemoveAsync(id);

            var getUserDTO = _mapper.Map<GetUserDTO>(user);
            await _kafkaProducerService.SendUserRequestAsync(RequestOperation.Remove, getUserDTO);

            _logger.LogInformation("user - {user} removed", JsonSerializer.Serialize(getUserDTO));
        }
    }
}
