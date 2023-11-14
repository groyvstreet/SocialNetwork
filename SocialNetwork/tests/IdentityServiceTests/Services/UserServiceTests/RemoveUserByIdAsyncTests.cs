using AutoMapper;
using IdentityService.BLL;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace IdentityServiceTests.Services.UserServiceTests
{
    public class RemoveUserByIdAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IKafkaProducerService<RequestOperation, GetUserDTO>> _kafkaProducerService;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IUserService _userService;

        public RemoveUserByIdAsyncTests()
        {
            _mapper = new Mock<IMapper>();
            _userRepository = new Mock<IUserRepository>();
            _kafkaProducerService = new Mock<IKafkaProducerService<RequestOperation, GetUserDTO>>();
            _logger = new Mock<ILogger<UserService>>();
            _userCacheRepository = new Mock<ICacheRepository<User>>();

            _userService = new UserService(_mapper.Object,
                _userRepository.Object,
                _kafkaProducerService.Object,
                _logger.Object,
                _userCacheRepository.Object);
        }

        [Fact]
        public async Task RemoveUserByIdAsyncTestThrowsForbidden()
        {
            // Arrange
            var id = "1";
            var authenticatedUserId = "2";
            var authenticatedUserRole = Roles.User;

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _userService.RemoveUserByIdAsync(id,
                authenticatedUserId,
                authenticatedUserRole));
        }

        [Theory]
        [InlineData("1", "1", Roles.Admin)]
        [InlineData("1", "2", Roles.Admin)]
        [InlineData("1", "1", Roles.User)]
        public async Task RemoveUserByIdAsyncTestThrowsNotFound(string id, string authenticatedUserId, string authenticatedUserRole)
        {
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.RemoveUserByIdAsync(id,
                authenticatedUserId,
                authenticatedUserRole));
        }

        [Theory]
        [InlineData("1", "1", Roles.Admin)]
        [InlineData("1", "2", Roles.Admin)]
        [InlineData("1", "1", Roles.User)]
        public async Task RemoveUserByIdAsyncTestWithGettingUserFromCache(string id,
            string authenticatedUserId,
            string authenticatedUserRole)
        {
            // Arrange
            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            // Act
            await _userService.RemoveUserByIdAsync(id, authenticatedUserId, authenticatedUserRole);

            // Assert
            _userRepository.Verify(userRepository => userRepository.GetUserByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("1", "1", Roles.Admin)]
        [InlineData("1", "2", Roles.Admin)]
        [InlineData("1", "1", Roles.User)]
        public async Task RemoveUserByIdAsyncTestWithGettingUserFromRepository(string id,
            string authenticatedUserId,
            string authenticatedUserRole)
        {
            // Arrange
            _userRepository.Setup(userCacheRepository => userCacheRepository.GetUserByIdAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            // Act
            await _userService.RemoveUserByIdAsync(id, authenticatedUserId, authenticatedUserRole);

            // Assert
            _userCacheRepository.Verify(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
