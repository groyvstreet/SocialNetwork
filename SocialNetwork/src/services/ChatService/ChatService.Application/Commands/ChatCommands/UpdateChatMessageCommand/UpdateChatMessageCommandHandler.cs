using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand
{
    public class UpdateChatMessageCommandHandler : IRequestHandler<UpdateChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ILogger<UpdateChatMessageCommandHandler> _logger;

        public UpdateChatMessageCommandHandler(IChatRepository chatRepository,
                                               IChatNotificationService chatNotificationService,
                                               ILogger<UpdateChatMessageCommandHandler> logger)
        {
            _chatRepository = chatRepository;
            _chatNotificationService = chatNotificationService;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateChatMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(chat => chat.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            var message = chat.Messages.FirstOrDefault(message => message.Id == DTO.MessageId);

            if (message is null)
            {
                throw new NotFoundException($"no such message with id = {DTO.MessageId}");
            }

            if (message.User.Id != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            await _chatRepository.UpdateChatMessageAsync(DTO.ChatId, DTO.MessageId, DTO.Text);

            await _chatNotificationService.UpdateMessageAsync(chat, message, DTO.Text);

            message.Text = DTO.Text;
            _logger.LogInformation("message - {message} updated in chat with id {id}", JsonSerializer.Serialize(message), chat.Id);

            return new Unit();
        }
    }
}
