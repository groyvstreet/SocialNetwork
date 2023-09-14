using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand
{
    public class UpdateDialogMessageCommandHandler : IRequestHandler<UpdateDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;

        public UpdateDialogMessageCommandHandler(IDialogRepository dialogRepository)
        {
            _dialogRepository = dialogRepository;
        }

        public async Task<Unit> Handle(UpdateDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d => d.Id == request.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {request.DialogId}");
            }

            // check message

            await _dialogRepository.UpdateDialogMessageAsync(request.DialogId, request.MessageId, request.Text);

            return new Unit();
        }
    }
}
