using AutoMapper;
using ChatService.Application.DTOs.DialogDTOs;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Queries.DialogQueries.GetDialogsQuery;
using MediatR;
using Moq;

namespace ChatServiceTests.Queries.DialogQueriesTests.GetDialogsQueryHandlerTests
{
    public class HandleTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IDialogRepository> _dialogRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly IRequestHandler<GetDialogsQuery, List<GetDialogDTO>> _getDialogsQueryHandler;

        public HandleTests()
        {
            _mapper = new Mock<IMapper>();
            _dialogRepository = new Mock<IDialogRepository>();
            _userRepository = new Mock<IUserRepository>();

            _getDialogsQueryHandler = new GetDialogsQueryHandler(_mapper.Object,
                _dialogRepository.Object,
                _userRepository.Object);
        }

        [Fact]
        public async Task HandleTestThrowsForbidden()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = Guid.NewGuid();
            var request = new GetDialogsQuery(userId, authenticatedUserId);

            await Assert.ThrowsAsync<ForbiddenException>(() => _getDialogsQueryHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsNotFound()
        {
            var userId = Guid.NewGuid();
            var authenticatedUserId = userId;
            var request = new GetDialogsQuery(userId, authenticatedUserId);

            await Assert.ThrowsAsync<NotFoundException>(() => _getDialogsQueryHandler.Handle(request, CancellationToken.None));
        }
    }
}
