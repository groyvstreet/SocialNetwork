using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PostService.Application.Interfaces.PostInterfaces;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Application.Interfaces;
using PostService.Application.Services;
using PostService.Domain.Entities;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Application.Exceptions;
using System.Linq.Expressions;

namespace PostServiceTests.Services.PostLikeServiceTests
{
    public class RemovePostLikeAsyncTests
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

        public RemovePostLikeAsyncTests()
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
        public async Task RemovePostLikeAsyncTestThrowsForbidden()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task RemovePostLikeAsyncTestThrowsNotFound()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, authenticatedUserId));
        }

        [Fact]
        public async Task RemovePostLikeAsyncTestWithPostLikeFromCacheWithPostFromCache()
        {
            _postLikeCacheRepository.Setup(postLikeCacheRepository => postLikeCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new PostLike());

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postLikeRepository.Verify(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()), Times.Never);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task RemovePostLikeAsyncTestWithPostLikeFromRepositoryWithPostFromCache()
        {
            _postLikeRepository.Setup(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()).Result)
                .Returns(new PostLike());

            _postCacheRepository.Setup(postCacheRepository => postCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new Post());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postLikeRepository.Verify(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()), Times.Once);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Never);
        }

        [Fact]
        public async Task RemovePostLikeAsyncTestWithPostLikeFromCacheWithPostFromRepostiry()
        {
            _postLikeCacheRepository.Setup(postLikeCacheRepository => postLikeCacheRepository.GetAsync(It.IsAny<string>()).Result)
                .Returns(new PostLike());

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postLikeRepository.Verify(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()), Times.Never);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task RemovePostLikeAsyncTestWithPostLikeFromRepositoryWithPostFromRepostiry()
        {
            _postLikeRepository.Setup(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()).Result)
                .Returns(new PostLike());

            _postRepository.Setup(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()).Result)
                .Returns(new Post());

            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO { UserId = userId };

            await _postLikeService.RemovePostLikeAsync(addRemovePostLikeDTO, authenticatedUserId);

            _postLikeRepository.Verify(postLikeRepository =>
                postLikeRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<PostLike, bool>>>()), Times.Once);

            _postRepository.Verify(postRepository =>
                postRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Post, bool>>>()), Times.Once);
        }
    }
}
