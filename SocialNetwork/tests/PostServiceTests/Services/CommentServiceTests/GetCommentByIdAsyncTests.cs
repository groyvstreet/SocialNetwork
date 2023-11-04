using AutoMapper;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces.CommentInterfaces;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using Moq;
using PostService.Application.Exceptions;
using PostService.Application.DTOs.CommentDTOs;
using FluentAssertions;

namespace PostServiceTests.Services.CommentServiceTests
{
    public class GetCommentByIdAsyncTests
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

        public GetCommentByIdAsyncTests()
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
        public async Task GetCommentByIdAsyncTestThrowsNotFound()
        {
            var id = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(() => _commentService.GetCommentByIdAsync(id));
        }

        [Fact]
        public async Task GetCommentByIdAsyncTestReturnsFromCache()
        {
            var id = Guid.NewGuid();
            var comment = new Comment { Id = id };

            _commentCacheRepository.Setup(commentCacheRepository => commentCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(comment);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>()))
                .Returns(Map);

            var resultComment = await _commentService.GetCommentByIdAsync(id);

            _commentRepository.Verify(commentRepository => commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == id),
                Times.Never);

            resultComment.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetCommentByIdAsyncTestReturnsFromRepository()
        {
            var id = Guid.NewGuid();
            var comment = new Comment { Id = id };

            _commentRepository.Setup(commentRepository => commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == id).Result)
                .Returns(comment);

            _mapper.Setup(mapper => mapper.Map<GetCommentDTO>(It.IsAny<Comment>()))
                .Returns(Map);

            var resultComment = await _commentService.GetCommentByIdAsync(id);

            _commentRepository.Verify(commentRepository => commentRepository.GetFirstOrDefaultByAsync(comment => comment.Id == id),
                Times.Once);

            resultComment.Id.Should().Be(id);
        }

        private GetCommentDTO Map(Comment comment)
        {
            return new GetCommentDTO { Id = comment.Id };
        }
    }
}
