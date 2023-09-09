using AutoMapper;
using IdentityService.BLL.DTOs.IdentityDTOs;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using System.Security.Claims;

namespace IdentityService.BLL.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly ITokenService tokenService;

        public IdentityService(IMapper mapper,
                               IUserRepository userRepository,
                               ITokenService tokenService)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.tokenService = tokenService;
        }

        public async Task<GetUserDTO> SignUpAsync(AddUserDTO addUserDTO)
        {
            var user = await userRepository.GetUserByEmailAsync(addUserDTO.Email);

            if (user is not null)
            {
                throw new AlreadyExistsException($"user with email = {addUserDTO.Email} already exists");
            }

            user = mapper.Map<User>(addUserDTO);
            user.UserName = user.Email;
            await userRepository.AddUserAsync(user, addUserDTO.Password, Roles.User);
            var getUserDTO = mapper.Map<GetUserDTO>(user);

            return getUserDTO;
        }

        public async Task<AuthenticatedResponseDTO> SignInAsync(string email, string password)
        {
            var user = await userRepository.GetUserByEmailAndPasswordAsync(email, password);

            if (user is null)
            {
                throw new UnauthorizedAccessException("email or password are invalid");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await userRepository.GetUserRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authenticatedResponseDTO = new AuthenticatedResponseDTO
            {
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id)
            };

            return authenticatedResponseDTO;
        }
    }
}
