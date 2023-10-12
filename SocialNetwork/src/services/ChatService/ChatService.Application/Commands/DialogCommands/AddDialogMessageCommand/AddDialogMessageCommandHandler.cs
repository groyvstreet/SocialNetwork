using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand
{
    public class AddDialogMessageCommandHandler : IRequestHandler<AddDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDialogNotificationService _dialogNotificationService;
        private readonly ICacheRepository<User> _userCacheRepository;

        public AddDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                              IUserRepository userRepository,
                                              IDialogNotificationService dialogNotificationService,
                                              ICacheRepository<User> userCacheRepository)
        {
            _dialogRepository = dialogRepository;
            _userRepository = userRepository;
            _dialogNotificationService = dialogNotificationService;
            _userCacheRepository = userCacheRepository;
        }

        public async Task<Unit> Handle(AddDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.SenderId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var sender = await _userCacheRepository.GetAsync(DTO.SenderId.ToString());

            if (sender is null)
            {
                sender = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.SenderId);

                if (sender is null)
                {
                    throw new NotFoundException($"no such user with id = {DTO.SenderId}");
                }

                await _userCacheRepository.SetAsync(sender.Id.ToString(), sender);
            }

            var receiver = await _userCacheRepository.GetAsync(DTO.ReceiverId.ToString());

            if (receiver is null)
            {
                receiver = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.ReceiverId);

                if (receiver is null)
                {
                    throw new NotFoundException($"no such user with id = {DTO.ReceiverId}");
                }

                await _userCacheRepository.SetAsync(receiver.Id.ToString(), receiver);
            }

            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d =>
                d.Users.Any(u => u.Id == DTO.SenderId) && d.Users.Any(u => u.Id == DTO.ReceiverId));

            if (dialog is null)
            {
                dialog = new Dialog();
                dialog.Users.AddRange(new List<User> { sender, receiver });
                await _dialogRepository.AddAsync(dialog);
            }

            var message = new Message
            {
                DateTime = DateTimeOffset.Now,
                Text = DTO.Text,
                User = sender
            };
            await _dialogRepository.AddDialogMessageAsync(dialog.Id, message);

            await _dialogNotificationService.SendMessageAsync(dialog, message);

            return new Unit();
        }
    }
}
