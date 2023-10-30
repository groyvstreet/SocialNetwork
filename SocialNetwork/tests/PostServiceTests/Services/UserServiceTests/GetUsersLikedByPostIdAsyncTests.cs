using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using System.Linq.Expressions;

namespace PostServiceTests.Services.UserServiceTests
{
    public class GetUsersLikedByPostIdAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<ICommentLikeRepository> _commentLikeRepository;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IPostLikeRepository> _postLikeRepository;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly Mock<ICacheRepository<Comment>> _commentCacheRepository;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly IUserService _userService;

        public GetUsersLikedByPostIdAsyncTests()
        {
            _mapper = new Mock<IMapper>();
            _commentRepository = new Mock<ICommentRepository>();
            _commentLikeRepository = new Mock<ICommentLikeRepository>();
            _postRepository = new Mock<IPostRepository>();
            _postLikeRepository = new Mock<IPostLikeRepository>();
            _logger = new Mock<ILogger<UserService>>();
            _commentCacheRepository = new Mock<ICacheRepository<Comment>>();
            _postCacheRepository = new Mock<ICacheRepository<Post>>();

            _userService = new UserService(_mapper.Object,
                _commentRepository.Object,
                _commentLikeRepository.Object,
                _postRepository.Object,
                _postLikeRepository.Object,
                _logger.Object,
                _commentCacheRepository.Object,
                _postCacheRepository.Object);
        }

        [Fact]
        public async Task GetUsersLikedByPostIdAsyncTestReturnsCommentFromCache()
        {
            var id = Guid.NewGuid();

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            _postLikeRepository.Setup(postLikeRepository =>
                postLikeRepository.GetPostLikesWithUserByPostIdAsync(id).Result)
                .Returns(new List<PostLike>());

            await _userService.GetUsersLikedByPostIdAsync(id);

            _postRepository.Verify(postRepository => postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task GetUsersLikedByPostIdAsyncTestReturnsCommentFromRepository()
        {
            var id = Guid.NewGuid();

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            _postLikeRepository.Setup(postLikeRepository =>
                postLikeRepository.GetPostLikesWithUserByPostIdAsync(id).Result)
                .Returns(new List<PostLike>());

            await _userService.GetUsersLikedByPostIdAsync(id);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task GetUsersLikedByPostIdAsyncTestThrowsNotFound()
        {
            var id = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUsersLikedByPostIdAsync(id));
        }
    }
}
