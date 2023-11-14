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
using PostService.Application.DTOs.CommentDTOs;
using FluentAssertions.Execution;
using FluentAssertions;
using System.Linq.Expressions;

namespace PostServiceTests.Services.CommentServiceTests
{
    public class AddCommentAsyncTests
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

        public AddCommentAsyncTests()
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
        public async Task AddCommentAsyncTestThrowsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var addCommentDTO = new AddCommentDTO { UserId = userId };
            var authenticatedUserId = Guid.NewGuid();

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddCommentAsyncTestThrowsPostNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;

            var addCommentDTO = new AddCommentDTO
            {
                Text = "cool",
                PostId = postId,
                UserId = userId
            };

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddCommentAsyncTestThrowsUserNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(post);

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;

            var addCommentDTO = new AddCommentDTO
            {
                Text = "cool",
                PostId = postId,
                UserId = userId
            };

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddCommentAsyncTestWithPostFromCacheWithUserFromCache()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(post);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(user);

            _mapper.Setup(mapper => mapper.Map<Comment>(It.IsAny<AddCommentDTO>()))
                .Returns((Func<AddCommentDTO, Comment>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>()))
                .Returns((Func<Comment, GetCommentDTO>)Map);

            var authenticatedUserId = userId;

            var addCommentDTO = new AddCommentDTO
            {
                Text = "cool",
                PostId = postId,
                UserId = userId
            };

            // Act
            var resultComment = await _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId);

            // Assert
            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            using (new AssertionScope())
            {
                resultComment.Text.Should().Be(addCommentDTO.Text);
                resultComment.PostId.Should().Be(addCommentDTO.PostId);
                resultComment.UserId.Should().Be(addCommentDTO.UserId);
            }
        }

        [Fact]
        public async Task AddCommentAsyncTestWithPostFromRepositoryWithUserFromCache()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(post);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(user);

            _mapper.Setup(mapper => mapper.Map<Comment>(It.IsAny<AddCommentDTO>()))
                .Returns((Func<AddCommentDTO, Comment>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>()))
                .Returns((Func<Comment, GetCommentDTO>)Map);

            var authenticatedUserId = userId;

            var addCommentDTO = new AddCommentDTO
            {
                Text = "cool",
                PostId = postId,
                UserId = userId
            };

            // Act
            var resultComment = await _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId);

            // Assert
            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            using (new AssertionScope())
            {
                resultComment.Text.Should().Be(addCommentDTO.Text);
                resultComment.PostId.Should().Be(addCommentDTO.PostId);
                resultComment.UserId.Should().Be(addCommentDTO.UserId);
            }
        }

        [Fact]
        public async Task AddCommentAsyncTestWithPostFromCacheWithUserFromRepository()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(post);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(user);

            _mapper.Setup(mapper => mapper.Map<Comment>(It.IsAny<AddCommentDTO>()))
                .Returns((Func<AddCommentDTO, Comment>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>()))
                .Returns((Func<Comment, GetCommentDTO>)Map);

            var authenticatedUserId = userId;

            var addCommentDTO = new AddCommentDTO
            {
                Text = "cool",
                PostId = postId,
                UserId = userId
            };

            // Act
            var resultComment = await _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId);

            // Assert
            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            using (new AssertionScope())
            {
                resultComment.Text.Should().Be(addCommentDTO.Text);
                resultComment.PostId.Should().Be(addCommentDTO.PostId);
                resultComment.UserId.Should().Be(addCommentDTO.UserId);
            }
        }

        [Fact]
        public async Task AddCommentAsyncTestWithPostFromRepositoryWithUserFromRepository()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new Post { Id = postId };

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(post);

            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(user);

            _mapper.Setup(mapper => mapper.Map<Comment>(It.IsAny<AddCommentDTO>()))
                .Returns((Func<AddCommentDTO, Comment>)Map);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>()))
                .Returns((Func<Comment, GetCommentDTO>)Map);

            var authenticatedUserId = userId;

            var addCommentDTO = new AddCommentDTO
            {
                Text = "cool",
                PostId = postId,
                UserId = userId
            };

            // Act
            var resultComment = await _commentService.AddCommentAsync(addCommentDTO, authenticatedUserId);

            // Assert
            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            using (new AssertionScope())
            {
                resultComment.Text.Should().Be(addCommentDTO.Text);
                resultComment.PostId.Should().Be(addCommentDTO.PostId);
                resultComment.UserId.Should().Be(addCommentDTO.UserId);
            }
        }

        private Comment Map(AddCommentDTO addCommentDTO)
        {
            return new Comment
            {
                Text = addCommentDTO.Text,
                PostId = addCommentDTO.PostId,
                UserId = addCommentDTO.UserId
            };
        }

        private GetCommentDTO Map(Comment comment)
        {
            return new GetCommentDTO
            {
                Id = comment.Id,
                Text = comment.Text,
                PostId = comment.PostId,
                UserId = comment.UserId,
                DateTime = comment.DateTime,
                LikeCount = comment.LikeCount
            };
        }
    }
}
