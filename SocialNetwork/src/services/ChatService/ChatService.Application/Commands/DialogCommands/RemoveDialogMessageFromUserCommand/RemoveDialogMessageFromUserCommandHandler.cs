using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand
{
    public class RemoveDialogMessageFromUserCommandHandler : IRequestHandler<RemoveDialogMessageFromUserCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly ILogger<RemoveDialogMessageFromUserCommandHandler> _logger;

        public RemoveDialogMessageFromUserCommandHandler(IDialogRepository dialogRepository,
                                                         ILogger<RemoveDialogMessageFromUserCommandHandler> logger)
        {
            _dialogRepository = dialogRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveDialogMessageFromUserCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(dialog => dialog.Id == DTO.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {DTO.DialogId}");
            }

            if (!dialog.Messages.Any(message => message.Id == DTO.MessageId))
            {
                throw new NotFoundException($"no such message with id = {DTO.MessageId}");
            }

            if (dialog.Messages.First(message => message.Id == DTO.MessageId).UsersRemoved.Any(user => user == DTO.UserId.ToString()))
            {
                throw new AlreadyExistsException($"message with id = {DTO.MessageId} is already removed from user with id = {DTO.UserId}");
            }

            if (dialog.Messages.First(message => message.Id == DTO.MessageId).UsersRemoved.Count ==
                dialog.Users.Count - 1)
            {
                if (dialog.MessageCount == 1)
                {
                    await _dialogRepository.RemoveAsync(dialog);
                }
                else
                {
                    await _dialogRepository.RemoveDialogMessageAsync(DTO.DialogId, DTO.MessageId);
                }
            }
            else
            {
                await _dialogRepository.RemoveDialogMessageFromUserAsync(DTO.DialogId, DTO.MessageId, DTO.UserId);
            }

            _logger.LogInformation("message - {message} removed for user {user} from dialog with id {id}",
                JsonSerializer.Serialize(dialog.Messages.First(message => message.Id == DTO.MessageId)),
                JsonSerializer.Serialize(dialog.Users.First(user => user.Id == DTO.UserId)),
                dialog.Id);

            return new Unit();
        }
    }
}
