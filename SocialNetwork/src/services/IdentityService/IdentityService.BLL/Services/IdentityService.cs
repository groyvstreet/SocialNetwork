using AutoMapper;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityService.BLL.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly IOptions<JwtOptions> jwtOptions;

        public IdentityService(IMapper mapper,
                               IUserRepository userRepository,
                               IOptions<JwtOptions> jwtOptions)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.jwtOptions = jwtOptions;
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

        public async Task<string> SignInByJwt(string email, string password)
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

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(jwtOptions.Value.LifeTime)),
                signingCredentials: new SigningCredentials(jwtOptions.Value.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
