using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using IdentityService.BLL;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace IdentityServiceTests.Services.IdentitySeviceTests
{
    public class SignInAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ITokenService> _tokenService;
        private readonly Mock<IKafkaProducerService<RequestOperation, GetUserDTO>> _kafkaProducerService;
        private readonly Mock<ILogger<IdentityService.BLL.Services.IdentityService>> _logger;
        private readonly IIdentityService _identityService;

        public SignInAsyncTests()
        {
            _mapper = new Mock<IMapper>();
            _userRepository = new Mock<IUserRepository>();
            _tokenService = new Mock<ITokenService>();
            _kafkaProducerService = new Mock<IKafkaProducerService<RequestOperation, GetUserDTO>>();
            _logger = new Mock<ILogger<IdentityService.BLL.Services.IdentityService>>();

            _identityService = new IdentityService.BLL.Services.IdentityService(_mapper.Object,
                _userRepository.Object,
                _tokenService.Object,
                _kafkaProducerService.Object,
                _logger.Object);
        }

        [Fact]
        public async Task SignInAsyncThrowsUnauthorizedAccess()
        {
            var email = "email";
            var password = "password";

            _userRepository.Setup(userRepository => userRepository.GetUserByEmailAndPasswordAsync(email, password).Result)
                .Returns((User?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _identityService.SignInAsync(email, password));
        }

        [Fact]
        public async Task SignInAsyncReturnsTokens()
        {
            var email = "email";
            var password = "password";

            _userRepository.Setup(userRepository => userRepository.GetUserByEmailAndPasswordAsync(email, password).Result)
                .Returns(new User());

            _userRepository.Setup(userRepository => userRepository.GetUserRolesAsync(It.IsAny<User>()).Result)
                .Returns(new List<string>());

            _tokenService.Setup(tokenService => tokenService.GenerateAccessToken(It.IsAny<List<Claim>>()))
                .Returns("access token");

            _tokenService.Setup(tokenService => tokenService.GenerateRefreshTokenAsync(It.IsAny<string>()).Result)
                .Returns("refresh token");

            var response = await _identityService.SignInAsync(email, password);

            using (new AssertionScope())
            {
                response.AccessToken.Should().NotBeNullOrEmpty();
                response.RefreshToken.Should().NotBeNullOrEmpty();
                response.RefreshToken.Should().NotBeEquivalentTo(response.AccessToken);
            }
        }
    }
}
