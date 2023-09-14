using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
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

            // check message

            await _dialogRepository.RemoveDialogMessageFromUserAsync(request.DialogId, request.MessageId, request.UserId);
            
            // check for removing of dialog
            
            return new Unit();
        }
    }
}
