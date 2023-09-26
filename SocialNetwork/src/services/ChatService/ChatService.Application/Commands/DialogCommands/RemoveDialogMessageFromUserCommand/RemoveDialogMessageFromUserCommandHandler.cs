using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand
{
    public class RemoveDialogMessageFromUserCommandHandler : IRequestHandler<RemoveDialogMessageFromUserCommand>
    {
        private readonly IDialogRepository _dialogRepository;

        public RemoveDialogMessageFromUserCommandHandler(IDialogRepository dialogRepository)
        {
            _dialogRepository = dialogRepository;
        }

        public async Task<Unit> Handle(RemoveDialogMessageFromUserCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d => d.Id == DTO.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {DTO.DialogId}");
            }

            if (!dialog.Messages.Any(m => m.Id == DTO.MessageId))
            {
                throw new NotFoundException($"no such message with id = {DTO.MessageId}");
            }

            if (dialog.Messages.First(m => m.Id == DTO.MessageId).UsersRemoved.Any(u => u == DTO.UserId.ToString()))
            {
                throw new AlreadyExistsException($"message with id = {DTO.MessageId} is already removed from user with id = {DTO.UserId}");
            }

            if (dialog.Messages.First(m => m.Id == DTO.MessageId).UsersRemoved.Count ==
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
            
            return new Unit();
        }
    }
}
