using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand
{
    public class AddDialogMessageCommandHandler : IRequestHandler<AddDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public AddDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                              IUserRepository userRepository,
                                              IHubContext<ChatHub, IChatHub> hubContext)
        {
            _dialogRepository = dialogRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(AddDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var sender = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.SenderId);

            if (sender is null)
            {
                throw new NotFoundException($"no such user with id = {request.SenderId}");
            }

            var receiver = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.ReceiverId);

            if (receiver is null)
            {
                throw new NotFoundException($"no such user with id = {request.ReceiverId}");
            }

            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d =>
                d.Users.Any(u => u.Id == request.SenderId) && d.Users.Any(u => u.Id == request.ReceiverId));

            if (dialog is null)
            {
                dialog = new Dialog();
                dialog.Users.AddRange(new List<User> { sender, receiver });
                await _dialogRepository.AddAsync(dialog);
            }

            var message = new Message
            {
                DateTime = DateTimeOffset.Now,
                Text = request.Text,
                User = sender
            };
            await _dialogRepository.AddDialogMessageAsync(dialog.Id, message);
            await _dialogRepository.UpdateFieldAsync(dialog, d => d.MessageCount, dialog.MessageCount + 1);
            
            dialog.Messages = new List<Message> { message };
            await _hubContext.Clients.User(sender.Id.ToString()).SendToUser(receiver.Id.ToString(), dialog);
            await _hubContext.Clients.User(receiver.Id.ToString()).SendToUser(receiver.Id.ToString(), dialog);

            return new Unit();
        }
    }
}
