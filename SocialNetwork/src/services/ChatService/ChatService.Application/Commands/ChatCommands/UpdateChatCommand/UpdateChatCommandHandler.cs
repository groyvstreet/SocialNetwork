using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatCommand
{
    public class UpdateChatCommandHandler : IRequestHandler<UpdateChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ILogger<UpdateChatCommandHandler> _logger;

        public UpdateChatCommandHandler(IChatRepository chatRepository,
                                        IChatNotificationService chatNotificationService,
                                        ILogger<UpdateChatCommandHandler> logger)
        {
            _chatRepository = chatRepository;
            _chatNotificationService = chatNotificationService;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateChatCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(chat => chat.Id == DTO.Id);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.Id}");
            }

            var user = chat.Users.FirstOrDefault(user => user.Id == request.AuthenticatedUserId);

            if (user is null || !user.IsAdmin)
            {
                throw new ForbiddenException("forbidden");
            }

            await _chatRepository.UpdateFieldAsync(chat, chata => chat.Name, DTO.Name);
            await _chatRepository.UpdateFieldAsync(chat, chat => chat.Image, DTO.Image);

            await _chatNotificationService.UpdateChatAsync(chat, DTO);

            chat.Name = DTO.Name;
            chat.Image = DTO.Image;
            chat.Users.Clear();
            chat.Messages.Clear();
            _logger.LogInformation("chat - {chat} updated", JsonSerializer.Serialize(chat));

            return new Unit();
        }
    }
}
