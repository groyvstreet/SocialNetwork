using AutoMapper;
using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Queries.ChatQueries.GetChatsQuery;
using MediatR;
using Moq;

namespace ChatServiceTests.Queries.ChatQueriesTests.GetChatsQueryHandlerTests
{
    public class HandleTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IChatRepository> _chatRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly IRequestHandler<GetChatsQuery, List<GetChatDTO>> _getChatsQueryHandler;

        public HandleTests()
        {
            _mapper = new Mock<IMapper>();
            _chatRepository = new Mock<IChatRepository>();
            _userRepository = new Mock<IUserRepository>();

            _getChatsQueryHandler = new GetChatsQueryHandler(_mapper.Object,
                _chatRepository.Object,
                _userRepository.Object);
        }

        [Fact]
        public async Task HandleTestThrowsForbidden()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();
            var request = new GetChatsQuery(userId, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => _getChatsQueryHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var request = new GetChatsQuery(userId, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _getChatsQueryHandler.Handle(request, CancellationToken.None));
        }
    }
}
