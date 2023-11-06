using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using IdentityService.BLL;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.BLL.Exceptions;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace IdentityServiceTests.Services.IdentitySeviceTests
{
    public class SignUpAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ITokenService> _tokenService;
        private readonly Mock<IKafkaProducerService<RequestOperation, GetUserDTO>> _kafkaProducerService;
        private readonly Mock<ILogger<IdentityService.BLL.Services.IdentityService>> _logger;
        private readonly IIdentityService _identityService;

        public SignUpAsyncTests()
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
        public async Task SignUpAsyncTestThrowsAlreadyExists()
        {
            _userRepository.Setup(userRepository => userRepository.GetUserByEmailAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            var addUserDTO = new AddUserDTO { Email = "email" };

            await Assert.ThrowsAsync<AlreadyExistsException>(() => _identityService.SignUpAsync(addUserDTO));
        }

        [Fact]
        public async Task SignUpAsyncTestReturnsUser()
        {
            _userRepository.Setup(userRepository => userRepository.GetUserByEmailAsync(It.IsAny<string>()).Result)
                .Returns((User?)null);

            _mapper.Setup(mapper => mapper.Map<User>(It.IsAny<AddUserDTO>()))
                .Returns(MapToUser);

            _mapper.Setup(mapper => mapper.Map<GetUserDTO>(It.IsAny<User>()))
                .Returns(MapToGetUserDTO);

            var addUserDTO = new AddUserDTO
            {
                Email = "email",
                FirstName = "first",
                LastName = "last",
                BirthDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var resultUser = await _identityService.SignUpAsync(addUserDTO);

            using (new AssertionScope())
            {
                resultUser.Email.Should().Be(addUserDTO.Email);
                resultUser.FirstName.Should().Be(addUserDTO.FirstName);
                resultUser.LastName.Should().Be(addUserDTO.LastName);
                resultUser.BirthDate.Should().Be(addUserDTO.BirthDate);
                resultUser.Image.Should().BeEmpty();
            }
        }

        private static User MapToUser(AddUserDTO addUserDTO)
        {
            return new User
            {
                Email = addUserDTO.Email,
                FirstName = addUserDTO.FirstName,
                LastName = addUserDTO.LastName,
                BirthDate = addUserDTO.BirthDate.ToDateTime(TimeOnly.MinValue)
            };
        }

        private static GetUserDTO MapToGetUserDTO(User user)
        {
            return new GetUserDTO
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = DateOnly.FromDateTime(user.BirthDate),
                Image = user.Image
            };
        }
    }
}
