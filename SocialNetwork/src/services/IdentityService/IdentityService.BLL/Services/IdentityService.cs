using AutoMapper;
using IdentityService.BLL.DTOs.IdentityDTOs;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace IdentityService.BLL.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public IdentityService(IMapper mapper,
                               IUserRepository userRepository,
                               ITokenService tokenService,
                               IKafkaProducerService kafkaProducerService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<GetUserDTO> SignUpAsync(AddUserDTO addUserDTO)
        {
            var user = await _userRepository.GetUserByEmailAsync(addUserDTO.Email);

            if (user is not null)
            {
                throw new AlreadyExistsException($"user with email = {addUserDTO.Email} already exists");
            }

            user = _mapper.Map<User>(addUserDTO);
            user.UserName = user.Email;
            await _userRepository.AddUserAsync(user, addUserDTO.Password, Roles.User);
            var getUserDTO = _mapper.Map<GetUserDTO>(user);
            
            await _kafkaProducerService.SendUserRequestAsync(UserRequest.Create, getUserDTO);

            return getUserDTO;
        }

        public async Task<AuthenticatedResponseDTO> SignInAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

            if (user is null)
            {
                throw new UnauthorizedAccessException("email or password are invalid");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userRepository.GetUserRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authenticatedResponseDTO = new AuthenticatedResponseDTO
            {
                AccessToken = _tokenService.GenerateAccessToken(claims),
                RefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.Id)
            };

            return authenticatedResponseDTO;
        }
    }
}
