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
using FluentAssertions;
using FluentAssertions.Execution;

namespace PostServiceTests.Services.PostServiceTests
{
    public class AddPostAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPostLikeRepository> _postLikeRepository;
        private readonly Mock<ILogger<PostService.Application.Services.PostService>> _logger;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly IPostService _postService;

        public AddPostAsyncTests()
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
        public async Task AddPostAsyncTestThrowsForbidden()
        {
            // Arrange
            var id = Guid.NewGuid();
            var addPostDTO = new AddPostDTO { UserId = id };
            var authenticatedUserId = Guid.NewGuid();

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _postService.AddPostAsync(addPostDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddPostAsyncTestThrowsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var addPostDTO = new AddPostDTO { UserId = id };
            var authenticatedUserId = id;

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _postService.AddPostAsync(addPostDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddPostAsyncTestWithUserFromCache()
        {
            // Arrange
            var id = Guid.NewGuid();
            var addPostDTO = new AddPostDTO { UserId = id };

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<Post>(It.IsAny<AddPostDTO>())).Returns((Func<AddPostDTO, Post>)Map);

            _mapper.Setup(mapper => mapper.Map<GetPostDTO>(It.IsAny<Post>())).Returns((Func<Post, GetPostDTO>)Map);

            // Act
            var resultPost = await _postService.AddPostAsync(addPostDTO, id);

            // Assert
            using (new AssertionScope())
            {
                resultPost.Text.Should().Be(addPostDTO.Text);
                resultPost.UserId.Should().Be(addPostDTO.UserId);
            }
        }

        [Fact]
        public async Task AddPostAsyncTestWithUserFromRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var addPostDTO = new AddPostDTO { UserId = id };

            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(user => user.Id == addPostDTO.UserId).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<Post>(It.IsAny<AddPostDTO>())).Returns((Func<AddPostDTO, Post>)Map);

            _mapper.Setup(mapper => mapper.Map<GetPostDTO>(It.IsAny<Post>())).Returns((Func<Post, GetPostDTO>)Map);

            // Act
            var resultPost = await _postService.AddPostAsync(addPostDTO, id);

            // Assert
            using (new AssertionScope())
            {
                resultPost.Text.Should().Be(addPostDTO.Text);
                resultPost.UserId.Should().Be(addPostDTO.UserId);
            }
        }

        private static Post Map(AddPostDTO addPostDTO)
        {
            return new Post
            {
                Text = addPostDTO.Text,
                UserId = addPostDTO.UserId
            };
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
