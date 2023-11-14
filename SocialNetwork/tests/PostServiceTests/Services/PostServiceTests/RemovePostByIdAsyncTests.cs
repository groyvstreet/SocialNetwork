using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Application.Exceptions;

namespace PostServiceTests.Services.PostServiceTests
{
    public class RemovePostByIdAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPostLikeRepository> _postLikeRepository;
        private readonly Mock<ILogger<PostService.Application.Services.PostService>> _logger;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IPostService _postService;

        public RemovePostByIdAsyncTests()
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
        public async Task RemovePostByIdAsyncTestThrowsNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _postService.RemovePostByIdAsync(postId, authenticatedUserId));
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestWithPostFromCache()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = postId,
                UserId = authenticatedUserId
            };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(post);

            // Act
            await _postService.RemovePostByIdAsync(postId, authenticatedUserId);

            // Assert
            _postRepository.Verify(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == postId), Times.Never);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestWithPostFromRepository()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = postId,
                UserId = authenticatedUserId
            };

            _postRepository.Setup(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == postId).Result)
                .Returns(post);

            // Act
            await _postService.RemovePostByIdAsync(postId, authenticatedUserId);

            // Assert
            _postRepository.Verify(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == postId), Times.Once);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestWithPostFromCacheThrowsFordbidden()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(post);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _postService.RemovePostByIdAsync(postId, authenticatedUserId));

            _postRepository.Verify(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == postId), Times.Never);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestWithPostFromRepositoryThrowsFordbidden()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _postRepository.Setup(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == postId).Result)
                .Returns(post);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _postService.RemovePostByIdAsync(postId, authenticatedUserId));

            _postRepository.Verify(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == postId), Times.Once);
        }
    }
}
