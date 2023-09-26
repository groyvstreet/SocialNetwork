using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatCommand
{
    public class UpdateChatCommandHandler : IRequestHandler<UpdateChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _chatNotificationService;

        public UpdateChatCommandHandler(IChatRepository chatRepository,
                                        IChatNotificationService chatNotificationService)
        {
            _chatRepository = chatRepository;
            _chatNotificationService = chatNotificationService;
        }

        public async Task<Unit> Handle(UpdateChatCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == DTO.Id);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.Id}");
            }

            var user = chat.Users.FirstOrDefault(u => u.Id == request.AuthenticatedUserId);

            if (user is null || !user.IsAdmin)
            {
                throw new ForbiddenException("forbidden");
            }

            await _chatRepository.UpdateFieldAsync(chat, c => c.Name, DTO.Name);
            await _chatRepository.UpdateFieldAsync(chat, c => c.Image, DTO.Image);

            await _chatNotificationService.UpdateChatAsync(chat, DTO);

            return new Unit();
        }
    }
}
