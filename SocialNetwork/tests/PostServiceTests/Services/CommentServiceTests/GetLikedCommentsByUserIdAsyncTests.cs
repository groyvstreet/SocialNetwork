using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Exceptions;
using FluentAssertions;
using PostService.Application.DTOs.CommentDTOs;

namespace PostServiceTests.Services.CommentServiceTests
{
    public class GetLikedCommentsByUserIdAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ICommentLikeRepository> _commentLikeRepository;
        private readonly Mock<ILogger<CommentService>> _logger;
        private readonly Mock<ICacheRepository<Comment>> _commentCacheRepository;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly ICommentService _commentService;

        public GetLikedCommentsByUserIdAsyncTests()
        {
            _mapper = new Mock<IMapper>();
            _commentRepository = new Mock<ICommentRepository>();
            _postRepository = new Mock<IPostRepository>();
            _userRepository = new Mock<IUserRepository>();
            _commentLikeRepository = new Mock<ICommentLikeRepository>();
            _logger = new Mock<ILogger<CommentService>>();
            _commentCacheRepository = new Mock<ICacheRepository<Comment>>();
            _postCacheRepository = new Mock<ICacheRepository<Post>>();
            _userCacheRepository = new Mock<ICacheRepository<User>>();

            _commentService = new CommentService(_mapper.Object,
                _commentRepository.Object,
                _postRepository.Object,
                _userRepository.Object,
                _commentLikeRepository.Object,
                _logger.Object,
                _commentCacheRepository.Object,
                _postCacheRepository.Object,
                _userCacheRepository.Object);
        }

        [Fact]
        public async Task GetLikedCommentsByUserIdAsyncTestThrowsNotFound()
        {
            var userId = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(() => _commentService.GetLikedCommentsByUserIdAsync(userId));
        }

        [Fact]
        public async Task GetLikedCommentsByUserIdAsyncTestWithUserFromCache()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(user);

            _commentLikeRepository.Setup(commentLikeRepository =>
                commentLikeRepository.GetCommentLikesWithCommentByUserIdAsync(userId).Result)
                .Returns(new List<CommentLike>());

            await _commentService.GetLikedCommentsByUserIdAsync(userId);

            _userRepository.Verify(userRepository => userRepository.GetFirstOrDefaultByAsync(user => user.Id == userId),
                Times.Never);
        }

        [Fact]
        public async Task GetLikedCommentsByUserIdAsyncTestWithUserFromRepository()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepository.Setup(userRepository => userRepository.GetFirstOrDefaultByAsync(user => user.Id == userId).Result)
                .Returns(user);

            _commentLikeRepository.Setup(commentLikeRepository =>
                commentLikeRepository.GetCommentLikesWithCommentByUserIdAsync(userId).Result)
                .Returns(new List<CommentLike>());

            await _commentService.GetLikedCommentsByUserIdAsync(userId);

            _userRepository.Verify(userRepository => userRepository.GetFirstOrDefaultByAsync(user => user.Id == userId),
                Times.Once);
        }
    }
}
