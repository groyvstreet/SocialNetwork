using ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand;
using ChatService.Application.DTOs.MessageDTOs;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace ChatServiceTests.Commands.DialogCommands.RemoveDialogMessageCommandHandlerTests
{
    public class HandleTests
    {
        private readonly Mock<IDialogRepository> _dialogRepository;
        private readonly Mock<IDialogNotificationService> _dialogNotificationService;
        private readonly Mock<ILogger<RemoveDialogMessageCommandHandler>> _logger;
        private readonly IRequestHandler<RemoveDialogMessageCommand> _removeDialogMessageCommandHandler;

        public HandleTests()
        {
            _dialogRepository = new Mock<IDialogRepository>();
            _dialogNotificationService = new Mock<IDialogNotificationService>();
            _logger = new Mock<ILogger<RemoveDialogMessageCommandHandler>>();

            _removeDialogMessageCommandHandler = new RemoveDialogMessageCommandHandler(_dialogRepository.Object,
                _dialogNotificationService.Object,
                _logger.Object);
        }

        [Fact]
        public async Task HandleTestThrowsDialogNotFound()
        {
            // Arrange
            var removeDialogMessageDTO = new RemoveDialogMessageDTO
            {
                DialogId = Guid.NewGuid(),
                MessageId = Guid.NewGuid()
            };

            var authenticatedUserId = Guid.NewGuid();
            var request = new RemoveDialogMessageCommand(removeDialogMessageDTO, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _removeDialogMessageCommandHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsMessageNotFound()
        {
            // Arrange
            _dialogRepository.Setup(dialogRepository =>
                dialogRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Dialog, bool>>>()).Result)
                .Returns(new Dialog());

            var removeDialogMessageDTO = new RemoveDialogMessageDTO
            {
                DialogId = Guid.NewGuid(),
                MessageId = Guid.NewGuid()
            };

            var authenticatedUserId = Guid.NewGuid();
            var request = new RemoveDialogMessageCommand(removeDialogMessageDTO, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _removeDialogMessageCommandHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsForbidden()
        {
            // Arrange
            var message = new Message
            {
                Id = Guid.NewGuid(),
                User = new User()
            };

            var dialog = new Dialog();
            dialog.Messages.Add(message);

            _dialogRepository.Setup(dialogRepository =>
                dialogRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Dialog, bool>>>()).Result)
                .Returns(dialog);

            var removeDialogMessageDTO = new RemoveDialogMessageDTO
            {
                DialogId = Guid.NewGuid(),
                MessageId = message.Id
            };

            var authenticatedUserId = Guid.NewGuid();
            var request = new RemoveDialogMessageCommand(removeDialogMessageDTO, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _removeDialogMessageCommandHandler.Handle(request, CancellationToken.None));
        }
    }
}
