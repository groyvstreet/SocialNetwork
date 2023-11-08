using ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand;
using ChatService.Application.DTOs.MessageDTOs;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace ChatServiceTests.Commands.DialogCommands.UpdateDialogMessageCommandHandlerTests
{
    public class HandleTests
    {
        private readonly Mock<IDialogRepository> _dialogRepository;
        private readonly Mock<IDialogNotificationService> _dialogNotificationService;
        private readonly Mock<ILogger<UpdateDialogMessageCommandHandler>> _logger;
        private readonly IRequestHandler<UpdateDialogMessageCommand> _updateDialogMessageCommandHandler;

        public HandleTests()
        {
            _dialogRepository = new Mock<IDialogRepository>();
            _dialogNotificationService = new Mock<IDialogNotificationService>();
            _logger = new Mock<ILogger<UpdateDialogMessageCommandHandler>>();

            _updateDialogMessageCommandHandler = new UpdateDialogMessageCommandHandler(_dialogRepository.Object,
                _dialogNotificationService.Object,
                _logger.Object);
        }

        [Fact]
        public async Task HandleTestThrowsDialogNotFound()
        {
            var updateDialogMessageDTO = new UpdateDialogMessageDTO();
            var authenticatedUserId = Guid.NewGuid();
            var request = new UpdateDialogMessageCommand(updateDialogMessageDTO, authenticatedUserId);

            await Assert.ThrowsAsync<NotFoundException>(() => _updateDialogMessageCommandHandler.Handle(request,
                CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsMessageNotFound()
        {
            _dialogRepository.Setup(dialogRepository =>
                dialogRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Dialog, bool>>>()).Result)
                .Returns(new Dialog());

            var updateDialogMessageDTO = new UpdateDialogMessageDTO();
            var authenticatedUserId = Guid.NewGuid();
            var request = new UpdateDialogMessageCommand(updateDialogMessageDTO, authenticatedUserId);

            await Assert.ThrowsAsync<NotFoundException>(() => _updateDialogMessageCommandHandler.Handle(request,
                CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsForbidden()
        {
            var userId = Guid.NewGuid();
            var message = new Message { User = new User { Id = userId } };

            var dialog = new Dialog();
            dialog.Messages.Add(message);

            _dialogRepository.Setup(dialogRepository =>
                dialogRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Dialog, bool>>>()).Result)
                .Returns(dialog);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                MessageId = message.Id
            };

            var authenticatedUserId = Guid.NewGuid();
            var request = new UpdateDialogMessageCommand(updateDialogMessageDTO, authenticatedUserId);

            await Assert.ThrowsAsync<ForbiddenException>(() => _updateDialogMessageCommandHandler.Handle(request,
                CancellationToken.None));
        }
    }
}
