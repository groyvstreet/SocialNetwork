using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand
{
    public class UpdateDialogMessageCommandHandler : IRequestHandler<UpdateDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IHubContext<DialogHub, IDialogHub> _hubContext;

        public UpdateDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                                 IHubContext<DialogHub, IDialogHub> hubContext)
        {
            _dialogRepository = dialogRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(UpdateDialogMessageCommand request, CancellationToken cancellationToken)
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

            await _dialogRepository.UpdateDialogMessageAsync(request.DialogId, request.MessageId, request.Text);

            dialog.Messages = new List<Message> { dialog.Messages.First(m => m.Id == request.MessageId) };
            dialog.Messages.First().Text = request.Text;
            var sender = dialog.Messages.First().User;
            var receiver = dialog.Users.First(u => u.Id != sender.Id);
            await _hubContext.Clients.User(sender.Id.ToString()).UpdateMessage(dialog);
            await _hubContext.Clients.User(receiver.Id.ToString()).UpdateMessage(dialog);

            return new Unit();
        }
    }
}
