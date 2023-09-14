using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand
{
    public class RemoveDialogMessageCommandHandler : IRequestHandler<RemoveDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;

        public RemoveDialogMessageCommandHandler(IDialogRepository dialogRepository)
        {
            _dialogRepository = dialogRepository;
        }

        public async Task<Unit> Handle(RemoveDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d => d.Id == request.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {request.DialogId}");
            }

            // check message

            await _dialogRepository.RemoveDialogMessageAsync(request.DialogId, request.MessageId);
            
            // check for removing of dialog
            
            await _dialogRepository.UpdateFieldAsync(dialog, d => d.MessageCount, dialog.MessageCount - 1);
            
            return new Unit();
        }
    }
}
