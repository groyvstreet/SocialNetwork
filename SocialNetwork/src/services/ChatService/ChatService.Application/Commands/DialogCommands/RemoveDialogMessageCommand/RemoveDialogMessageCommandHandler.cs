using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand
{
    public class RemoveDialogMessageCommandHandler : IRequestHandler<RemoveDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IHubContext<DialogHub, IDialogHub> _hubContext;

        public RemoveDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                                 IHubContext<DialogHub, IDialogHub> hubContext)
        {
            _dialogRepository = dialogRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(RemoveDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d => d.Id == request.DialogId);

            if (dialog is null)
            {
                throw new NotFoundException($"no such dialog with id = {request.DialogId}");
            }

            var message = dialog.Messages.FirstOrDefault(m => m.Id == request.MessageId);

            if (message is null)
            {
                throw new NotFoundException($"no such message with id = {request.MessageId}");
            }

            if (message.User.Id != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            dialog.Messages = new List<Message> { dialog.Messages.First(m => m.Id == request.MessageId) };
            var sender = dialog.Messages.First().User;
            var receiver = dialog.Users.First(u => u.Id != sender.Id);

            if (dialog.MessageCount == 1)
            {
                await _dialogRepository.RemoveAsync(dialog);

                dialog.Messages = new List<Message>();
                dialog.MessageCount = 0;
                await _hubContext.Clients.User(sender.Id.ToString()).RemoveDialog(dialog);
                await _hubContext.Clients.User(receiver.Id.ToString()).RemoveDialog(dialog);
            }
            else
            {
                await _dialogRepository.RemoveDialogMessageAsync(request.DialogId, request.MessageId);
                await _dialogRepository.UpdateFieldAsync(dialog, d => d.MessageCount, dialog.MessageCount - 1);

                dialog.MessageCount--;
                await _hubContext.Clients.User(sender.Id.ToString()).RemoveMessage(dialog);
                await _hubContext.Clients.User(receiver.Id.ToString()).RemoveMessage(dialog);
            }
            
            return new Unit();
        }
    }
}
