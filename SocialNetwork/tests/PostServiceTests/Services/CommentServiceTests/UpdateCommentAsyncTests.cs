using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using FluentAssertions.Execution;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces.PostInterfaces;
using FluentAssertions;

namespace CommentServiceTests.Services.CommentServiceTests
{
    public class UpdateCommentAsyncTests
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

        public UpdateCommentAsyncTests()
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
        public async Task UpdateCommentAsyncTestThrowsNotFound()
        {
            var commentId = Guid.NewGuid();
            var updateCommentDTO = new UpdateCommentDTO { Id = commentId };
            var authenticatedUserId = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _commentService.UpdateCommentAsync(updateCommentDTO, authenticatedUserId));
        }

        [Fact]
        public async Task UpdateCommentAsyncTestWithCommentFromCache()
        {
            var commentId = Guid.NewGuid();
            var updateCommentDTO = new UpdateCommentDTO { Id = commentId };
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                UserId = authenticatedUserId
            };

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(comment);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>())).Returns(Map);

            var resultComment = await _commentService.UpdateCommentAsync(updateCommentDTO, authenticatedUserId);

            using (new AssertionScope())
            {
                resultComment.Id.Should().Be(updateCommentDTO.Id);
                resultComment.Text.Should().Be(updateCommentDTO.Text);
            }
        }

        [Fact]
        public async Task UpdateCommentAsyncTestWithCommentFromRepository()
        {
            var commentId = Guid.NewGuid();
            var updateCommentDTO = new UpdateCommentDTO { Id = commentId };
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = commentId,
                UserId = authenticatedUserId
            };

            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == updateCommentDTO.Id).Result)
                .Returns(comment);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>())).Returns(Map);

            var resultComment = await _commentService.UpdateCommentAsync(updateCommentDTO, authenticatedUserId);

            using (new AssertionScope())
            {
                resultComment.Id.Should().Be(updateCommentDTO.Id);
                resultComment.Text.Should().Be(updateCommentDTO.Text);
            }
        }

        [Fact]
        public async Task UpdateCommentAsyncTestWithCommentFromCacheThrowsFordbidden()
        {
            var commentId = Guid.NewGuid();
            var updateCommentDTO = new UpdateCommentDTO { Id = commentId };
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(comment);

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _commentService.UpdateCommentAsync(updateCommentDTO, authenticatedUserId));
        }

        [Fact]
        public async Task UpdateCommentAsyncTestWithCommentFromRepositoryThrowsFordbidden()
        {
            var commentId = Guid.NewGuid();
            var updateCommentDTO = new UpdateCommentDTO { Id = commentId };
            var authenticatedUserId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _commentRepository.Setup(commentRepository =>
                commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == updateCommentDTO.Id).Result)
                .Returns(comment);

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _commentService.UpdateCommentAsync(updateCommentDTO, authenticatedUserId));
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
