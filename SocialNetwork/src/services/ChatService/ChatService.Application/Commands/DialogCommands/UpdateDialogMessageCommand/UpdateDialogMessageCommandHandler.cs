using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand
{
    public class UpdateDialogMessageCommandHandler : IRequestHandler<UpdateDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IDialogNotificationService _dialogNotificationService;

        public UpdateDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                                 IDialogNotificationService dialogNotificationService)
        {
            _dialogRepository = dialogRepository;
            _dialogNotificationService = dialogNotificationService;
        }

        public async Task<Unit> Handle(UpdateDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d => d.Id == DTO.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {DTO.DialogId}");
            }

            var message = dialog.Messages.FirstOrDefault(m => m.Id == DTO.MessageId);

            if (message is null)
            {
                throw new NotFoundException($"no such message with id = {DTO.MessageId}");
            }

            if (message.User.Id != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            await _dialogRepository.UpdateDialogMessageAsync(DTO.DialogId, DTO.MessageId, DTO.Text);

            await _dialogNotificationService.UpdateMessageAsync(dialog, message, DTO.Text);

            return new Unit();
        }
    }
}
