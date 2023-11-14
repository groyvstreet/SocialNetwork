using ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand;
using ChatService.Application.DTOs.MessageDTOs;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace ChatServiceTests.Commands.DialogCommands.RemoveDialogMessageFromUserCommandHandlerTests
{
    public class HandleTests
    {
        private readonly Mock<IDialogRepository> _dialogRepository;
        private readonly Mock<ILogger<RemoveDialogMessageFromUserCommandHandler>> _logger;
        private readonly IRequestHandler<RemoveDialogMessageFromUserCommand> _removeDialogMessageFromUserCommandHandler;

        public HandleTests()
        {
            _dialogRepository = new Mock<IDialogRepository>();
            _logger = new Mock<ILogger<RemoveDialogMessageFromUserCommandHandler>>();

            _removeDialogMessageFromUserCommandHandler = new RemoveDialogMessageFromUserCommandHandler(_dialogRepository.Object,
                _logger.Object);
        }

        [Fact]
        public async Task HandleTestThrowsForbidden()
        {
            // Arrange
            var removeDialogMessageFromUserDTO = new RemoveDialogMessageFromUserDTO
            {
                DialogId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var authenticatedUserId = Guid.NewGuid();
            var request = new RemoveDialogMessageFromUserCommand(removeDialogMessageFromUserDTO, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _removeDialogMessageFromUserCommandHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsDialogNotFound()
        {
            // Arrange
            var removeDialogMessageFromUserDTO = new RemoveDialogMessageFromUserDTO
            {
                DialogId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var authenticatedUserId = removeDialogMessageFromUserDTO.UserId;
            var request = new RemoveDialogMessageFromUserCommand(removeDialogMessageFromUserDTO, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _removeDialogMessageFromUserCommandHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsMessageNotFound()
        {
            // Arrange
            _dialogRepository.Setup(dialogRepository =>
                dialogRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Dialog, bool>>>()).Result)
                .Returns(new Dialog());

            var removeDialogMessageFromUserDTO = new RemoveDialogMessageFromUserDTO
            {
                DialogId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var authenticatedUserId = removeDialogMessageFromUserDTO.UserId;
            var request = new RemoveDialogMessageFromUserCommand(removeDialogMessageFromUserDTO, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _removeDialogMessageFromUserCommandHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task HandleTestThrowsAlreadyExists()
        {
            // Arrange
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId };

            var userId = Guid.NewGuid();
            message.UsersRemoved.Add(userId.ToString());

            var dialog = new Dialog();
            dialog.Messages.Add(message);

            _dialogRepository.Setup(dialogRepository =>
                dialogRepository.GetFirstOrDefaultByAsync(It.IsAny<Expression<Func<Dialog, bool>>>()).Result)
                .Returns(dialog);

            var removeDialogMessageFromUserDTO = new RemoveDialogMessageFromUserDTO
            {
                DialogId = Guid.NewGuid(),
                MessageId = messageId,
                UserId = userId
            };

            var authenticatedUserId = userId;
            var request = new RemoveDialogMessageFromUserCommand(removeDialogMessageFromUserDTO, authenticatedUserId);

            // Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() =>
                _removeDialogMessageFromUserCommandHandler.Handle(request, CancellationToken.None));
        }
    }
}
