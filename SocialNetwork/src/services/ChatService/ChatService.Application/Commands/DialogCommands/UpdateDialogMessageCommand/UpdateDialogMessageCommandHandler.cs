using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand
{
    public class UpdateDialogMessageCommandHandler : IRequestHandler<UpdateDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IDialogNotificationService _dialogNotificationService;
        private readonly ILogger<UpdateDialogMessageCommandHandler> _logger;

        public UpdateDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                                 IDialogNotificationService dialogNotificationService,
                                                 ILogger<UpdateDialogMessageCommandHandler> logger)
        {
            _dialogRepository = dialogRepository;
            _dialogNotificationService = dialogNotificationService;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(dialog => dialog.Id == DTO.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {DTO.DialogId}");
            }

            var message = dialog.Messages.FirstOrDefault(message => message.Id == DTO.MessageId);

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

            _logger.LogInformation("message - {message} updated in dialog with id {id}", JsonSerializer.Serialize(message), dialog.Id);

            return new Unit();
        }
    }
}
