using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Application.Exceptions;
using System.Linq.Expressions;

namespace CommentServiceTests.Services.CommentLikeServiceTests
{
    public class RemoveCommentLikeAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<ICommentLikeRepository> _commentLikeRepository;
        private readonly Mock<ICommentRepository> _commentRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ILogger<CommentLikeService>> _logger;
        private readonly Mock<ICacheRepository<Comment>> _commentCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly Mock<ICacheRepository<CommentLike>> _commentLikeCacheRepository;
        private readonly ICommentLikeService _commentLikeService;

        public RemoveCommentLikeAsyncTests()
        {
            _mapper = new Mock<IMapper>();
            _commentLikeRepository = new Mock<ICommentLikeRepository>();
            _commentRepository = new Mock<ICommentRepository>();
            _userRepository = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<CommentLikeService>>();
            _commentCacheRepository = new Mock<ICacheRepository<Comment>>();
            _userCacheRepository = new Mock<ICacheRepository<User>>();
            _commentLikeCacheRepository = new Mock<ICacheRepository<CommentLike>>();

            _commentLikeService = new CommentLikeService(_mapper.Object,
                _commentLikeRepository.Object,
                _commentRepository.Object,
                _userRepository.Object,
                _logger.Object,
                _commentCacheRepository.Object,
                _userCacheRepository.Object,
                _commentLikeCacheRepository.Object);
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestThrowsForbidden()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestThrowsNotFound()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestWithCommentLikeFromCacheWithCommentFromCache()
        {
            _commentLikeCacheRepository.Setup(commentLikeCacheRepository =>
                commentLikeCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new CommentLike());

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Comment());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            await _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            _commentLikeRepository.Verify(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()), Times.Never);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestWithCommentLikeFromRepositoryWithCommentFromCache()
        {
            _commentLikeRepository.Setup(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()).Result)
                .Returns(new CommentLike());

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Comment());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            await _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            _commentLikeRepository.Verify(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()), Times.Once);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestWithCommentLikeFromCacheWithCommentFromRecommentiry()
        {
            _commentLikeCacheRepository.Setup(commentLikeCacheRepository =>
                commentLikeCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new CommentLike());

            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()).Result)
                .Returns(new Comment());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            await _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            _commentLikeRepository.Verify(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()), Times.Never);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestWithCommentLikeFromRepositoryWithCommentFromRecommentiry()
        {
            _commentLikeRepository.Setup(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()).Result)
                .Returns(new CommentLike());

            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()).Result)
                .Returns(new Comment());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            await _commentLikeService.RemoveCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            _commentLikeRepository.Verify(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()), Times.Once);

            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Once);
        }
    }
}
