using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand
{
    internal class RemoveChatMessageFromUserCommandHandler : IRequestHandler<RemoveChatMessageFromUserCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<RemoveChatMessageFromUserCommandHandler> _logger;

        public RemoveChatMessageFromUserCommandHandler(IChatRepository chatRepository,
                                                       ILogger<RemoveChatMessageFromUserCommandHandler> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveChatMessageFromUserCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var chat = await _chatRepository.GetFirstOrDefaultByAsync(chat => chat.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            if (!chat.Messages.Any(message => message.Id == DTO.MessageId))
            {
                throw new NotFoundException($"no such message with id = {DTO.MessageId}");
            }

            if (!chat.Users.Any(user => user.Id == DTO.UserId))
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            await _chatRepository.RemoveChatMessageFromUserAsync(DTO.ChatId, DTO.MessageId, DTO.UserId);

            _logger.LogInformation("message - {message} removed from chat with id {id}",
                JsonSerializer.Serialize(chat.Messages.First(message => message.Id == DTO.MessageId)),
                chat.Id);

            return new Unit();
        }
    }
}
