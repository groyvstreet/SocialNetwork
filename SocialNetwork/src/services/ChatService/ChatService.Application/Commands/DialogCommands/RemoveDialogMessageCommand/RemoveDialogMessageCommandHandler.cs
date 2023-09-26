using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand
{
    public class RemoveDialogMessageCommandHandler : IRequestHandler<RemoveDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IDialogNotificationService _dialogNotificationService;

        public RemoveDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                                 IDialogNotificationService dialogNotificationService)
        {
            _dialogRepository = dialogRepository;
            _dialogNotificationService = dialogNotificationService;
        }

        public async Task<Unit> Handle(RemoveDialogMessageCommand request, CancellationToken cancellationToken)
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

            if (dialog.MessageCount == 1)
            {
                await _dialogRepository.RemoveAsync(dialog);

                await _dialogNotificationService.RemoveDialogAsync(dialog);
            }
            else
            {
                await _dialogRepository.RemoveDialogMessageAsync(DTO.DialogId, DTO.MessageId);

                await _dialogNotificationService.RemoveMessageAsync(dialog, message);
            }
            
            return new Unit();
        }
    }
}
