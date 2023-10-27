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
    public class UpdateUserAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IKafkaProducerService<RequestOperation, GetUserDTO>> _kafkaProducerService;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IUserService _userService;

        public UpdateUserAsyncTests()
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
        public async Task UpdateUserAsyncTestThrowsForbidden()
        {
            var updateUserDTO = new UpdateUserDTO { Id = "1" };
            var authenticatedUserId = "2";
            var authenticatedUserRole = Roles.User;

            await Assert.ThrowsAsync<ForbiddenException>(() => _userService.UpdateUserAsync(updateUserDTO,
                authenticatedUserId,
                authenticatedUserRole));
        }

        [Theory]
        [InlineData("1", "1", Roles.Admin)]
        [InlineData("1", "2", Roles.Admin)]
        [InlineData("1", "1", Roles.User)]
        public async Task UpdateUserAsyncTestThrowsNotFound(string id, string authenticatedUserId, string authenticatedUserRole)
        {
            var updateUserDTO = new UpdateUserDTO { Id = id };

            await Assert.ThrowsAsync<NotFoundException>(() => _userService.UpdateUserAsync(updateUserDTO,
                authenticatedUserId,
                authenticatedUserRole));
        }

        [Theory]
        [InlineData("1", "1", Roles.Admin)]
        [InlineData("1", "2", Roles.Admin)]
        [InlineData("1", "1", Roles.User)]
        public async Task UpdateUserAsyncTestWithGettingUserFromCache(string id, string authenticatedUserId, string authenticatedUserRole)
        {
            var user = new User { Id = id };

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(id).Result)
                .Returns(user);

            _mapper.Setup(mapper => mapper.Map<GetUserDTO>(It.IsAny<User>()))
                .Returns(Map);

            var updateUserDTO = new UpdateUserDTO
            {
                Id = id,
                FirstName = "First",
                LastName = "Last",
                BirthDate = DateOnly.FromDateTime(DateTime.Now),
                Image = "Image"
            };

            var resultUser = await _userService.UpdateUserAsync(updateUserDTO, authenticatedUserId, authenticatedUserRole);

            Assert.Equal(updateUserDTO.Id, resultUser.Id);
            Assert.Equal(updateUserDTO.FirstName, resultUser.FirstName);
            Assert.Equal(updateUserDTO.LastName, resultUser.LastName);
            Assert.Equal(updateUserDTO.BirthDate, resultUser.BirthDate);
            Assert.Equal(updateUserDTO.Image, resultUser.Image);
        }

        [Theory]
        [InlineData("1", "1", Roles.Admin)]
        [InlineData("1", "2", Roles.Admin)]
        [InlineData("1", "1", Roles.User)]
        public async Task UpdateUserAsyncTestWithGettingUserFromRepository(string id,
            string authenticatedUserId,
            string authenticatedUserRole)
        {
            var user = new User { Id = id };

            _userRepository.Setup(userCacheRepository => userCacheRepository.GetUserByIdAsync(id).Result)
                .Returns(user);

            _mapper.Setup(mapper => mapper.Map<GetUserDTO>(It.IsAny<User>()))
                .Returns(Map);

            var updateUserDTO = new UpdateUserDTO
            {
                Id = id,
                FirstName = "First",
                LastName = "Last",
                BirthDate = DateOnly.FromDateTime(DateTime.Now),
                Image = "Image"
            };

            var resultUser = await _userService.UpdateUserAsync(updateUserDTO, authenticatedUserId, authenticatedUserRole);

            Assert.Equal(updateUserDTO.Id, resultUser.Id);
            Assert.Equal(updateUserDTO.FirstName, resultUser.FirstName);
            Assert.Equal(updateUserDTO.LastName, resultUser.LastName);
            Assert.Equal(updateUserDTO.BirthDate, resultUser.BirthDate);
            Assert.Equal(updateUserDTO.Image, resultUser.Image);
        }

        private static GetUserDTO Map(User user)
        {
            return new GetUserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = DateOnly.FromDateTime(user.BirthDate),
                Image = user.Image
            };
        }
    }
}
