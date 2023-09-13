using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.MessageCommands.MessageCommandHandlers
{
    public class AddDialogMessageCommandHandler : IRequestHandler<AddDialogMessageCommand, Message>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IBaseRepository<User> _userRepository;

        public AddDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                              IBaseRepository<User> userRepository)
        {
            _dialogRepository = dialogRepository;
            _userRepository = userRepository;
        }

        public async Task<Message> Handle(AddDialogMessageCommand command, CancellationToken cancellationToken)
        {
            var dialog = await _dialogRepository.GetDialogByUsers(command.SenderId, command.ReceiverId);

            if (dialog is null)
            {
                dialog = new Dialog
                {
                    Users = new List<User>
                    {
                        new User { Id = command.SenderId },
                        new User { Id = command.ReceiverId }
                    },
                    Messages = new List<Message>()
                };
                await _dialogRepository.AddAsync(dialog);
            }

            var message = new Message();
            await _dialogRepository.AddMessageToDialogAsync(dialog.Id, message);

            return message;
        }
    }
}
