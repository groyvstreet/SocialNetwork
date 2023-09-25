using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand
{
    public class SetUserAsDefaultCommandHandler : IRequestHandler<SetUserAsDefaultCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _chatNotificationService;

        public SetUserAsDefaultCommandHandler(IChatRepository chatRepository,
                                              IChatNotificationService chatNotificationService)
        {
            _chatRepository = chatRepository;
            _chatNotificationService = chatNotificationService;
        }

        public async Task<Unit> Handle(SetUserAsDefaultCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            var authenticatedUser = chat.Users.FirstOrDefault(u => u.Id == request.AuthenticatedUserId);

            if (authenticatedUser is null || !authenticatedUser.IsAdmin)
            {
                throw new ForbiddenException("forbidden");
            }

            var user = chat.Users.First(u => u.Id == DTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            await _chatRepository.SetUserAsChatAdminAsync(DTO.ChatId, DTO.UserId, false);

            await _chatNotificationService.SetUserAsChatAdminAsync(chat, user, false);

            return new Unit();
        }
    }
}
