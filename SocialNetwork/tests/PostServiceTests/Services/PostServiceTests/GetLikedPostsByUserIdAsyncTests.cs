using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using System.Linq.Expressions;
using PostService.Application.Exceptions;

namespace PostServiceTests.Services.PostServiceTests
{
    public class GetLikedPostsByUserIdAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPostLikeRepository> _postLikeRepository;
        private readonly Mock<ILogger<PostService.Application.Services.PostService>> _logger;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IPostService _postService;

        public GetLikedPostsByUserIdAsyncTests()
        {
            _mapper = new Mock<IMapper>();
            _postRepository = new Mock<IPostRepository>();
            _userRepository = new Mock<IUserRepository>();
            _postLikeRepository = new Mock<IPostLikeRepository>();
            _logger = new Mock<ILogger<PostService.Application.Services.PostService>>();
            _postCacheRepository = new Mock<ICacheRepository<Post>>();
            _userCacheRepository = new Mock<ICacheRepository<User>>();

            _postService = new PostService.Application.Services.PostService(_mapper.Object,
                _postRepository.Object,
                _userRepository.Object,
                _postLikeRepository.Object,
                _logger.Object,
                _postCacheRepository.Object,
                _userCacheRepository.Object);
        }

        [Fact]
        public async Task GetLikedPostsByUserIdAsyncTestWithUserFromCache()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new User { Id = id };

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(id.ToString()).Result)
                .Returns(user);

            _postLikeRepository.Setup(postLikeRepository => postLikeRepository.GetPostLikesWithPostByUserIdAsync(id).Result)
                .Returns(new List<PostLike>());

            // Act
            await _postService.GetLikedPostsByUserIdAsync(id);

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task GetLikedPostsByUserIdAsyncTestWithUserFromRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new User { Id = id };

            _userRepository.Setup(userRepository => userRepository.GetFirstOrDefaultByAsync(user => user.Id == id).Result)
                .Returns(user);

            _postLikeRepository.Setup(postLikeRepository => postLikeRepository.GetPostLikesWithPostByUserIdAsync(id).Result)
                .Returns(new List<PostLike>());

            // Act
            await _postService.GetLikedPostsByUserIdAsync(id);

            // Assert
            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task GetLikedPostsByUserIdAsyncTestThrowsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _postService.GetLikedPostsByUserIdAsync(id));
        }
    }
}
