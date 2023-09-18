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

            if (!dialog.Messages.Any(m => m.Id == request.MessageId))
            {
                throw new NotFoundException($"no such message with id = {request.MessageId}");
            }
            
            if (dialog.MessageCount == 1)
            {
                await _dialogRepository.RemoveAsync(dialog);
            }
            else
            {
                await _dialogRepository.RemoveDialogMessageAsync(request.DialogId, request.MessageId);
                await _dialogRepository.UpdateFieldAsync(dialog, d => d.MessageCount, dialog.MessageCount - 1);
            }
            
            return new Unit();
        }
    }
}
