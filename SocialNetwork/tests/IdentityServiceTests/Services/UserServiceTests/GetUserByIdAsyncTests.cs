using AutoMapper;
using FluentAssertions;
using IdentityService.BLL;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.BLL.Services;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace IdentityServiceTests.Services.UserServiceTests
{
    public class GetUserByIdAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IKafkaProducerService<RequestOperation, GetUserDTO>> _kafkaProducerService;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IUserService _userService;

        public GetUserByIdAsyncTests()
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
        public async Task GetUserByIdAsyncTestReturnsUserFromCache()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var user = new User { Id = id };

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(id).Result)
                .Returns(user);

            var getUserDTO = new GetUserDTO { Id = id };

            _mapper.Setup(mapper => mapper.Map<GetUserDTO>(user))
                .Returns(getUserDTO);

            // Act
            var resultUser = await _userService.GetUserByIdAsync(id);

            // Assert
            resultUser.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetUserByIdAsyncTestReturnsUserFromRepository()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var user = new User { Id = id };

            _userRepository.Setup(userRepository => userRepository.GetUserByIdAsync(id).Result)
                .Returns(user);

            var getUserDTO = new GetUserDTO { Id = id };

            _mapper.Setup(mapper => mapper.Map<GetUserDTO>(user))
                .Returns(getUserDTO);

            // Act
            var resultUser = await _userService.GetUserByIdAsync(id);

            // Assert
            resultUser.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetUserByIdAsyncTestThrowsNotFound()
        {
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserByIdAsync(It.IsAny<string>()));
        }
    }
}
