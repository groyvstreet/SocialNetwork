using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using System.Linq.Expressions;
using FluentAssertions;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Exceptions;

namespace PostServiceTests.Services.PostServiceTests
{
    public class GetPostByIdAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPostLikeRepository> _postLikeRepository;
        private readonly Mock<ILogger<PostService.Application.Services.PostService>> _logger;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IPostService _postService;

        public GetPostByIdAsyncTests()
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
        public async Task GetPostByIdTestReturnsFromCache()
        {
            var id = Guid.NewGuid();
            var post = new Post { Id = id };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(id.ToString()).Result)
                .Returns(post);

            _mapper.Setup(mapper => mapper.Map<GetPostDTO>(It.IsAny<Post>()))
                .Returns(Map);

            var resultPost = await _postService.GetPostByIdAsync(id);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);

            resultPost.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetPostByIdTestReturnsFromRepository()
        {
            var id = Guid.NewGuid();
            var post = new Post { Id = id };

            _postRepository.Setup(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == id).Result)
                .Returns(post);

            _mapper.Setup(mapper => mapper.Map<GetPostDTO>(It.IsAny<Post>()))
                .Returns(Map);

            var resultPost = await _postService.GetPostByIdAsync(id);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);

            resultPost.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetPostByIdTestThrowsNotFound()
        {
            var id = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(() => _postService.GetPostByIdAsync(id));
        }

        private static GetPostDTO Map(Post post)
        {
            return new GetPostDTO { Id = post.Id };
        }
    }
}
