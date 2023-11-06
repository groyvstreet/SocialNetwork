using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Exceptions;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using System.Linq.Expressions;

namespace PostServiceTests.Services.PostLikeServiceTests
{
    public class AddPostLikeAsyncTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IPostLikeRepository> _postLikeRepository;
        private readonly Mock<IPostRepository> _postRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ILogger<PostLikeService>> _logger;
        private readonly Mock<ICacheRepository<Post>> _postCacheRepository;
        private readonly Mock<ICacheRepository<User>> _userCacheRepository;
        private readonly Mock<ICacheRepository<PostLike>> _postLikeCacheRepository;
        private readonly IPostLikeService _postLikeService;

        public AddPostLikeAsyncTests()
        {
            _mapper = new Mock<IMapper>();
            _postLikeRepository = new Mock<IPostLikeRepository>();
            _postRepository = new Mock<IPostRepository>();
            _userRepository = new Mock<IUserRepository>();
            _logger = new Mock<ILogger<PostLikeService>>();
            _postCacheRepository = new Mock<ICacheRepository<Post>>();
            _userCacheRepository = new Mock<ICacheRepository<User>>();
            _postLikeCacheRepository = new Mock<ICacheRepository<PostLike>>();

            _postLikeService = new PostLikeService(_mapper.Object,
                _postLikeRepository.Object,
                _postRepository.Object,
                _userRepository.Object,
                _logger.Object,
                _postCacheRepository.Object,
                _userCacheRepository.Object,
                _postLikeCacheRepository.Object);
        }

        [Fact]
        public async Task AddPostLikeAsyncTestThrowsForbidden()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddPostLikeAsyncTestThrowsPostNotFound()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddPostLikeAsyncTestThrowsUserNotFound()
        {
            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task AddPostLikeAsyncTestWithPostLikeFromCacheThrowsAlreadyExists()
        {
            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _postLikeCacheRepository.Setup(postLikeCacheRepository => postLikeCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new PostLike());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId));

            _postLikeRepository.Verify(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task AddPostLikeAsyncTestWithPostLikeFromRepositoryThrowsAlreadyExists()
        {
            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _postLikeRepository.Setup(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()).Result)
                .Returns(new PostLike());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId));

            _postLikeRepository.Verify(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task AddPostLikeAsyncTestWithPostFromCacheWithUserFromCache()
        {
            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<PostLike>(It.IsAny<AddRemovePostLikeDTO>()))
                .Returns((Func<AddRemovePostLikeDTO, PostLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetPostLikeDTO>(It.IsAny<PostLike>()))
                .Returns((Func<PostLike, GetPostLikeDTO>)Map);

            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO
            {
                PostId = postId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            var resultPostLike = await _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            using (new AssertionScope())
            {
                resultPostLike.PostId.Should().Be(addRemovePostLikeDTO.PostId);
                resultPostLike.UserId.Should().Be(addRemovePostLikeDTO.UserId);
            }
        }

        [Fact]
        public async Task AddPostLikeAsyncTestWithPostFromRepositoryWithUserFromCache()
        {
            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            _userCacheRepository.Setup(userCacheRepository => userCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<PostLike>(It.IsAny<AddRemovePostLikeDTO>()))
                .Returns((Func<AddRemovePostLikeDTO, PostLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetPostLikeDTO>(It.IsAny<PostLike>()))
                .Returns((Func<PostLike, GetPostLikeDTO>)Map);

            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO
            {
                PostId = postId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            var resultPostLike = await _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Never);

            using (new AssertionScope())
            {
                resultPostLike.PostId.Should().Be(addRemovePostLikeDTO.PostId);
                resultPostLike.UserId.Should().Be(addRemovePostLikeDTO.UserId);
            }
        }

        [Fact]
        public async Task AddPostLikeAsyncTestWithPostFromCacheWithUserFromRepository()
        {
            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<PostLike>(It.IsAny<AddRemovePostLikeDTO>()))
                .Returns((Func<AddRemovePostLikeDTO, PostLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetPostLikeDTO>(It.IsAny<PostLike>()))
                .Returns((Func<PostLike, GetPostLikeDTO>)Map);

            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO
            {
                PostId = postId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            var resultPostLike = await _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            using (new AssertionScope())
            {
                resultPostLike.PostId.Should().Be(addRemovePostLikeDTO.PostId);
                resultPostLike.UserId.Should().Be(addRemovePostLikeDTO.UserId);
            }
        }

        [Fact]
        public async Task AddPostLikeAsyncTestWithPostFromRepositoryWithUserFromRepository()
        {
            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            _userRepository.Setup(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()).Result)
                .Returns(new User());

            _mapper.Setup(mapper => mapper.Map<PostLike>(It.IsAny<AddRemovePostLikeDTO>()))
                .Returns((Func<AddRemovePostLikeDTO, PostLike>)Map);

            _mapper.Setup(mapper => mapper.Map<GetPostLikeDTO>(It.IsAny<PostLike>()))
                .Returns((Func<PostLike, GetPostLikeDTO>)Map);

            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO
            {
                PostId = postId,
                UserId = userId
            };

            var authenticatedUserId = userId;

            var resultPostLike = await _postLikeService.AddPostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);

            _userRepository.Verify(userRepository =>
                userRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);

            using (new AssertionScope())
            {
                resultPostLike.PostId.Should().Be(addRemovePostLikeDTO.PostId);
                resultPostLike.UserId.Should().Be(addRemovePostLikeDTO.UserId);
            }
        }

        private PostLike Map(AddRemovePostLikeDTO addRemovePostLikeDTO)
        {
            return new PostLike
            {
                PostId = addRemovePostLikeDTO.PostId,
                UserId = addRemovePostLikeDTO.UserId
            };
        }

        private GetPostLikeDTO Map(PostLike postLike)
        {
            return new GetPostLikeDTO
            {
                Id = postLike.Id,
                PostId = postLike.PostId,
                UserId = postLike.UserId
            };
        }
    }
}
