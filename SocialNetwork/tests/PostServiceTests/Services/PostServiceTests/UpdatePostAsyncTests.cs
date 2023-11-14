using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Application.DTOs.PostDTOs;
using PostService.Application.Exceptions;
using FluentAssertions.Execution;
using FluentAssertions;

namespace PostServiceTests.Services.PostServiceTests
{
    public class UpdatePostAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPostLikeRepository> _postLikeRepository;
        private readonly Mock<ILogger<PostService.Application.Services.PostService>> _logger;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IPostService _postService;

        public UpdatePostAsyncTests()
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
        public async Task UpdatePostAsyncTestThrowsNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var updatePostDTO = new UpdatePostDTO { Id = postId };
            var authenticatedUserId = Guid.NewGuid();

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _postService.UpdatePostAsync(updatePostDTO, authenticatedUserId));
        }

        [Fact]
        public async Task UpdatePostAsyncTestWithPostFromCache()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var updatePostDTO = new UpdatePostDTO { Id = postId };
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = postId,
                UserId = authenticatedUserId
            };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(post);

            _mapper.Setup(mapper => mapper.Map<GetPostDTO>(It.IsAny<Post>())).Returns(Map);

            // Act
            var resultPost = await _postService.UpdatePostAsync(updatePostDTO, authenticatedUserId);

            // Assert
            using (new AssertionScope())
            {
                resultPost.Id.Should().Be(updatePostDTO.Id);
                resultPost.Text.Should().Be(updatePostDTO.Text);
            }
        }

        [Fact]
        public async Task UpdatePostAsyncTestWithPostFromRepository()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var updatePostDTO = new UpdatePostDTO { Id = postId };
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = postId,
                UserId = authenticatedUserId
            };

            _postRepository.Setup(postRepository => postRepository.GetFirstOrDefaultByAsync(post => post.Id == updatePostDTO.Id).Result)
                .Returns(post);

            _mapper.Setup(mapper => mapper.Map<GetPostDTO>(It.IsAny<Post>())).Returns(Map);

            // Act
            var resultPost = await _postService.UpdatePostAsync(updatePostDTO, authenticatedUserId);

            // Assert
            using (new AssertionScope())
            {
                resultPost.Id.Should().Be(updatePostDTO.Id);
                resultPost.Text.Should().Be(updatePostDTO.Text);
            }
        }

        [Fact]
        public async Task UpdatePostAsyncTestWithPostFromCacheThrowsFordbidden()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var updatePostDTO = new UpdatePostDTO { Id = postId };
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(post);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _postService.UpdatePostAsync(updatePostDTO, authenticatedUserId));
        }

        [Fact]
        public async Task UpdatePostAsyncTestWithPostFromRepositoryThrowsFordbidden()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var updatePostDTO = new UpdatePostDTO { Id = postId };
            var authenticatedUserId = Guid.NewGuid();

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(post => post.Id == updatePostDTO.Id).Result)
                .Returns(post);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _postService.UpdatePostAsync(updatePostDTO, authenticatedUserId));
        }

        private static GetPostDTO Map(Post post)
        {
            return new GetPostDTO
            {
                Id = post.Id,
                Text = post.Text,
                UserId = post.UserId
            };
        }
    }
}
