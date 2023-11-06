using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using System.Linq.Expressions;

namespace CommentServiceTests.Services.CommentServiceTests
{
    public class RemoveCommentByIdAsyncTests
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

        public RemoveCommentByIdAsyncTests()
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
        public async Task RemoveCommentByIdAsyncTestThrowsNotFound()
        {
            var commentId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(() => _commentService.RemoveCommentByIdAsync(commentId, authenticatedUserId));
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestWithCommentFromCacheWithPostFromCache()
        {
            var commentId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                UserId = authenticatedUserId
            };

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(comment);

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            await _commentService.RemoveCommentByIdAsync(commentId, authenticatedUserId);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId), Times.Never);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestWithCommentFromRepositoryWithPostFromCache()
        {
            var commentId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                UserId = authenticatedUserId
            };

            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId).Result)
                .Returns(comment);

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            await _commentService.RemoveCommentByIdAsync(commentId, authenticatedUserId);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId), Times.Once);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestWithCommentFromCacheWithPostFromRepository()
        {
            var commentId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                UserId = authenticatedUserId
            };

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(comment);

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            await _commentService.RemoveCommentByIdAsync(commentId, authenticatedUserId);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId), Times.Never);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestWithCommentFromRepositoryWithPostFromRepository()
        {
            var commentId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                UserId = authenticatedUserId
            };

            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId).Result)
                .Returns(comment);

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            await _commentService.RemoveCommentByIdAsync(commentId, authenticatedUserId);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId), Times.Once);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestWithCommentFromCacheThrowsFordbidden()
        {
            var commentId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(comment);

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _commentService.RemoveCommentByIdAsync(commentId, authenticatedUserId));

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId), Times.Never);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestWithCommentFromRepositoryThrowsFordbidden()
        {
            var commentId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId).Result)
                .Returns(comment);

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _commentService.RemoveCommentByIdAsync(commentId, authenticatedUserId));

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == commentId), Times.Once);
        }
    }
}
