using FluentAssertions;
using IdentityService.BLL;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace IdentityServiceTests.Services.TokenServiceTests
{
    public class GenerateRefreshTokenAsyncTests
    {
        private readonly Mock<IOptions<JwtOptions>> _jwtOptions;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository;
        private readonly ITokenService _tokenService;

        public GenerateRefreshTokenAsyncTests()
        {
            _jwtOptions = new Mock<IOptions<JwtOptions>>();
            _refreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _tokenService = new TokenService(_jwtOptions.Object, _refreshTokenRepository.Object);
        }

        [Fact]
        public async Task GenerateRefreshTokenAsyncTestsWithoutRemoving()
        {
            var userId = Guid.NewGuid().ToString();

            _refreshTokenRepository.Setup(refreshTokenRepository =>
                refreshTokenRepository.GetRefreshTokenByUserIdAsync(userId).Result)
                    .Returns((RefreshToken?)null);

            var jwtOptions = new JwtOptions { RefreshTokenLifeTime = 4320 };

            _jwtOptions.SetupGet(jwtOptions => jwtOptions.Value)
                .Returns(jwtOptions);

            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(userId);

            _refreshTokenRepository.Verify(refreshTokenRepository => refreshTokenRepository.Remove(It.IsAny<RefreshToken>()),
                Times.Never);

            refreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GenerateRefreshTokenAsyncTestsWithRemoving()
        {
            var userId = Guid.NewGuid().ToString();

            _refreshTokenRepository.Setup(refreshTokenRepository =>
                refreshTokenRepository.GetRefreshTokenByUserIdAsync(userId).Result)
                    .Returns(new RefreshToken());

            var jwtOptions = new JwtOptions { RefreshTokenLifeTime = 4320 };

            _jwtOptions.SetupGet(jwtOptions => jwtOptions.Value)
                .Returns(jwtOptions);

            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(userId);

            _refreshTokenRepository.Verify(refreshTokenRepository => refreshTokenRepository.Remove(It.IsAny<RefreshToken>()),
                Times.Once);

            refreshToken.Should().NotBeEmpty();
        }
    }
}
