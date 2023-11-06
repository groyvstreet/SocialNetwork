using FluentAssertions;
using Grpc.Core;
using Moq;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Domain.Entities;
using System.Linq.Expressions;

namespace PostServiceTests.Grpc.PostServiceTests
{
    public class IsPostExistsAsyncTests
    {
        private readonly Mock<IPostRepository> _postRepository;
        private readonly PostService.Application.Grpc.Services.PostService _postService;
        private readonly ServerCallContext _serverCallContext;

        public IsPostExistsAsyncTests()
        {
            _postRepository = new Mock<IPostRepository>();

            _postService = new PostService.Application.Grpc.Services.PostService(_postRepository.Object);

            _serverCallContext = new Mock<ServerCallContext>().Object;
        }

        [Fact]
        public async Task IsPostExistsAsyncTestReturnsTrue()
        {
            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            var postId = Guid.NewGuid();
            var request = new PostService.Application.Grpc.Protos.Request { PostId = postId.ToString() };

            var reply = await _postService.IsPostExists(request, _serverCallContext);

            reply.IsPostExists.Should().Be(true);
        }

        [Fact]
        public async Task IsPostExistsAsyncTestReturnsFalse()
        {
            var postId = Guid.NewGuid();
            var request = new PostService.Application.Grpc.Protos.Request { PostId = postId.ToString() };

            var reply = await _postService.IsPostExists(request, _serverCallContext);

            reply.IsPostExists.Should().Be(false);
        }
    }
}
