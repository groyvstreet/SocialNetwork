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
            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d => d.Id == request.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {request.DialogId}");
            }

            if (!dialog.Messages.Any(m => m.Id == request.MessageId))
            {
                throw new NotFoundException($"no such message with id = {request.MessageId}");
            }

            if (dialog.Messages.First(m => m.Id == request.MessageId).UsersRemoved.Any(u => u == request.UserId.ToString()))
            {
                throw new AlreadyExistsException($"message with id = {request.MessageId} is already removed from user with id = {request.UserId}");
            }

            if (dialog.Messages.First(m => m.Id == request.MessageId).UsersRemoved.Count ==
                dialog.Users.Count - 1)
            {
                if (dialog.MessageCount == 1)
                {
                    await _dialogRepository.RemoveAsync(dialog);
                }
                else
                {
                    await _dialogRepository.RemoveDialogMessageAsync(request.DialogId, request.MessageId);
                    await _dialogRepository.UpdateFieldAsync(dialog, d => d.MessageCount, dialog.MessageCount - 1);
                }
            }
            else
            {
                await _dialogRepository.RemoveDialogMessageFromUserAsync(request.DialogId, request.MessageId, request.UserId);
            }
            
            return new Unit();
        }
    }
}
