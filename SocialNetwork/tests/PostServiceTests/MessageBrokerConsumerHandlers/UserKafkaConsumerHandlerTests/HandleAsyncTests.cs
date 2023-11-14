using Moq;
using PostService.Application;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Interfaces;
using PostService.Infrastructure.Services;
using System.Linq.Expressions;

namespace PostServiceTests.MessageBrokerConsumerHandlers.UserKafkaConsumerHandlerTests
{
    public class HandleAsyncTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IKafkaConsumerHandler<RequestOperation, User> _userKafkaConsumerHandler;

        public HandleAsyncTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _userCacheRepository = new Mock<ICacheRepository<User>>();

            _userKafkaConsumerHandler = new UserKafkaConsumerHandler(_userRepository.Object, _userCacheRepository.Object);
        }

        [Fact]
        public async Task HandleAsyncTestCreateRequestOperation()
        {
            // Act
            await _userKafkaConsumerHandler.HandleAsync(RequestOperation.Create, new User());

            // Assert
            _userRepository.Verify(userRepository => userRepository.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsyncTestUpdateRequestOperationWithUserFromCache()
        {
            // Arrange
            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            // Act
            await _userKafkaConsumerHandler.HandleAsync(RequestOperation.Update, new User());

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository => userRepository.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsyncTestUpdateRequestOperationWithUserFromRepository()
        {
            // Arrange
            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(new User());

            // Act
            await _userKafkaConsumerHandler.HandleAsync(RequestOperation.Update, new User());

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository => userRepository.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsyncTestUpdateRequestOperationWhenUserNotFound()
        {
            // Act
            await _userKafkaConsumerHandler.HandleAsync(RequestOperation.Update, new User());

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository => userRepository.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsyncTestRemoveRequestOperationWithUserFromCache()
        {
            // Arrange
            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            // Act
            await _userKafkaConsumerHandler.HandleAsync(RequestOperation.Remove, new User());

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository => userRepository.Remove(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsyncTestRemoveRequestOperationWithUserFromRepository()
        {
            // Arrange
            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(new User());

            // Act
            await _userKafkaConsumerHandler.HandleAsync(RequestOperation.Remove, new User());

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository => userRepository.Remove(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsyncTestRemoveRequestOperationWhenUserNotFound()
        {
            // Act
            await _userKafkaConsumerHandler.HandleAsync(RequestOperation.Remove, new User());

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultAsNoTrackingByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository => userRepository.Remove(It.IsAny<User>()), Times.Never);
        }
    }
}
