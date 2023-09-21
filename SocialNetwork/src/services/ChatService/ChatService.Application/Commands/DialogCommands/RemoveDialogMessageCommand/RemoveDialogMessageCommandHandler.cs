using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand
{
    public class RemoveDialogMessageCommandHandler : IRequestHandler<RemoveDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public RemoveDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                                 IHubContext<ChatHub, IChatHub> hubContext)
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

            if (!dialog.Messages.Any(m => m.Id == request.MessageId))
            {
                throw new NotFoundException($"no such message with id = {request.MessageId}");
            }

            dialog.Messages = new List<Message> { dialog.Messages.First(m => m.Id == request.MessageId) };
            var sender = dialog.Messages.First().User;
            var receiver = dialog.Users.First(u => u.Id != sender.Id);

            if (dialog.MessageCount == 1)
            {
                await _dialogRepository.RemoveAsync(dialog);

                dialog.Messages = new List<Message>();
                await _hubContext.Clients.User(sender.Id.ToString()).RemoveDialog(receiver.Id.ToString(), dialog);
                await _hubContext.Clients.User(receiver.Id.ToString()).RemoveDialog(receiver.Id.ToString(), dialog);
            }
            else
            {
                await _dialogRepository.RemoveDialogMessageAsync(request.DialogId, request.MessageId);
                await _dialogRepository.UpdateFieldAsync(dialog, d => d.MessageCount, dialog.MessageCount - 1);

                await _hubContext.Clients.User(sender.Id.ToString()).RemoveMessage(receiver.Id.ToString(), dialog);
                await _hubContext.Clients.User(receiver.Id.ToString()).RemoveMessage(receiver.Id.ToString(), dialog);
            }
            
            return new Unit();
        }
    }
}
