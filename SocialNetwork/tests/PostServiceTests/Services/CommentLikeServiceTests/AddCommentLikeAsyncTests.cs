using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using Moq;
using FluentAssertions.Execution;
using PostService.Application.DTOs.CommentLikeDTOs;
using System.Linq.Expressions;
using PostService.Application.Exceptions;
using FluentAssertions;

namespace CommentServiceTests.Services.CommentLikeServiceTests
{
    public class AddCommentLikeAsyncTests
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

        public AddCommentLikeAsyncTests()
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
        public async Task AddCommentLikeAsyncTestThrowsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestThrowsCommentNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestThrowsUserNotFound()
        {
            // Arrange
            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Comment());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestWithCommentLikeFromCacheThrowsAlreadyExists()
        {
            // Arrange
            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Comment());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _commentLikeCacheRepository.Setup(commentLikeCacheRepository =>
                commentLikeCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new CommentLike());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            // Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId));

            _commentLikeRepository.Verify(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestWithCommentLikeFromRepositoryThrowsAlreadyExists()
        {
            // Arrange
            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Comment());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _commentLikeRepository.Setup(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()).Result)
                .Returns(new CommentLike());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO { UserId = userId };

            // Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId));

            _commentLikeRepository.Verify(commentLikeRepository =>
                commentLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<CommentLike, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestWithCommentFromCacheWithUserFromCache()
        {
            // Arrange
            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Comment());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<CommentLike>(It.IsAny<AddRemoveCommentLikeDTO>()))
                .Returns((Func<AddRemoveCommentLikeDTO, CommentLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentLikeDTO>(It.IsAny<CommentLike>()))
                .Returns((Func<CommentLike, GetCommentLikeDTO>)Map);

            var commentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO
            {
                CommentId = commentId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            // Act
            var resultCommentLike = await _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            // Assert
            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            using (new AssertionScope())
            {
                resultCommentLike.CommentId.Should().Be(addRemoveCommentLikeDTO.CommentId);
                resultCommentLike.UserId.Should().Be(addRemoveCommentLikeDTO.UserId);
            }
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestWithCommentFromRepositoryWithUserFromCache()
        {
            // Arrange
            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()).Result)
                .Returns(new Comment());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<CommentLike>(It.IsAny<AddRemoveCommentLikeDTO>()))
                .Returns((Func<AddRemoveCommentLikeDTO, CommentLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentLikeDTO>(It.IsAny<CommentLike>()))
                .Returns((Func<CommentLike, GetCommentLikeDTO>)Map);

            var commentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO
            {
                CommentId = commentId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            // Act
            var resultCommentLike = await _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            // Assert
            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            using (new AssertionScope())
            {
                resultCommentLike.CommentId.Should().Be(addRemoveCommentLikeDTO.CommentId);
                resultCommentLike.UserId.Should().Be(addRemoveCommentLikeDTO.UserId);
            }
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestWithCommentFromCacheWithUserFromRepository()
        {
            // Arrange
            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Comment());

            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<CommentLike>(It.IsAny<AddRemoveCommentLikeDTO>()))
                .Returns((Func<AddRemoveCommentLikeDTO, CommentLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentLikeDTO>(It.IsAny<CommentLike>()))
                .Returns((Func<CommentLike, GetCommentLikeDTO>)Map);

            var commentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO
            {
                CommentId = commentId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            // Act
            var resultCommentLike = await _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            // Assert
            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            using (new AssertionScope())
            {
                resultCommentLike.CommentId.Should().Be(addRemoveCommentLikeDTO.CommentId);
                resultCommentLike.UserId.Should().Be(addRemoveCommentLikeDTO.UserId);
            }
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestWithCommentFromRepositoryWithUserFromRepository()
        {
            // Arrange
            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()).Result)
                .Returns(new Comment());

            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<CommentLike>(It.IsAny<AddRemoveCommentLikeDTO>()))
                .Returns((Func<AddRemoveCommentLikeDTO, CommentLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentLikeDTO>(It.IsAny<CommentLike>()))
                .Returns((Func<CommentLike, GetCommentLikeDTO>)Map);

            var commentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO
            {
                CommentId = commentId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            // Act
            var resultCommentLike = await _commentLikeService.AddCommentLikeAsync(addRemoveCommentLikeDTO, authenticatedUserId);

            // Assert
            _commentRepository.Verify(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Comment, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            using (new AssertionScope())
            {
                resultCommentLike.CommentId.Should().Be(addRemoveCommentLikeDTO.CommentId);
                resultCommentLike.UserId.Should().Be(addRemoveCommentLikeDTO.UserId);
            }
        }

        private CommentLike Map(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO)
        {
            return new CommentLike
            {
                CommentId = addRemoveCommentLikeDTO.CommentId,
                UserId = addRemoveCommentLikeDTO.UserId
            };
        }

        private GetCommentLikeDTO Map(CommentLike commentLike)
        {
            return new GetCommentLikeDTO
            {
                Id = commentLike.Id,
                CommentId = commentLike.CommentId,
                UserId = commentLike.UserId
            };
        }
    }
}
