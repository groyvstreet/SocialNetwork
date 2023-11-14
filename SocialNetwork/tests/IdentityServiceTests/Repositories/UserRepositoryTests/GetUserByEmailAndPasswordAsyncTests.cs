using FluentAssertions;
using FluentAssertions.Execution;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using IdentityService.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace IdentityServiceTests.Repositories.UserRepositoryTests
{
    public class GetUserByEmailAndPasswordAsyncTests
    {
        private readonly Mock<UserManager<User>> _userManager;
        private readonly IUserRepository _userRepository;

        public GetUserByEmailAndPasswordAsyncTests()
        {
            _userManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(),
                null!, null!, null!, null!, null!, null!, null!, null!);

            _userRepository = new UserRepository(_userManager.Object);
        }

        [Fact]
        public async Task GetUserByEmailAndPasswordAsyncTestWithEmailReturnsNull()
        {
            // Arrange
            var email = "email";
            var password = "password";

            _userManager.Setup(userManager => userManager.FindByEmailAsync(email).Result)
                .Returns((User?)null);

            // Act
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

            // Assert
            user.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmailAndPasswordAsyncTestWithPasswordReturnsNull()
        {
            // Arrange
            var email = "email";
            var password = "password";

            _userManager.Setup(userManager => userManager.FindByEmailAsync(email).Result)
                .Returns(new User());

            _userManager.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), password).Result)
                .Returns(false);

            // Act
            var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

            // Assert
            user.Should().BeNull();
        }

        [Fact]
        public async Task GetUserByEmailAndPasswordAsyncTestReturnsNotNull()
        {
            // Arrange
            var email = "email";
            var password = "password";
            var user = new User { Email = email };

            _userManager.Setup(userManager => userManager.FindByEmailAsync(email).Result)
                .Returns(user);

            _userManager.Setup(userManager => userManager.CheckPasswordAsync(It.IsAny<User>(), password).Result)
                .Returns(true);

            // Act
            var resultUser = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

            // Assert
            using (new AssertionScope())
            {
                resultUser.Should().NotBeNull();
                resultUser?.Email.Should().Be(email);
            }
        }
    }
}
