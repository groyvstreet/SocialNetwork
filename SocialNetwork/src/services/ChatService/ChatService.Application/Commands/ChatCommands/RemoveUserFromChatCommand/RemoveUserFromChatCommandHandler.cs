using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand
{
    public class RemoveUserFromChatCommandHandler : IRequestHandler<RemoveUserFromChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ILogger<RemoveUserFromChatCommandHandler> _logger;

        public RemoveUserFromChatCommandHandler(IChatRepository chatRepository,
                                                IChatNotificationService chatNotificationService,
                                                ILogger<RemoveUserFromChatCommandHandler> logger)
        {
            _chatRepository = chatRepository;
            _chatNotificationService = chatNotificationService;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveUserFromChatCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(chat => chat.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            var authenticatedUser = chat.Users.FirstOrDefault(user => user.Id == request.AuthenticatedUserId);

            if (authenticatedUser is null)
            {
                throw new NotFoundException($"no such user with id = {request.AuthenticatedUserId} in chat with id = {DTO.ChatId}");
            }

            if (!authenticatedUser.InvitedUsers.Any(user => user == DTO.UserId.ToString()) && !authenticatedUser.IsAdmin)
            {
                throw new ForbiddenException("forbidden");
            }

            var user = chat.Users.FirstOrDefault(user=> user.Id == DTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            await _chatRepository.RemoveUserFromChatAsync(DTO.ChatId, DTO.UserId);

            await _chatNotificationService.RemoveUserFromChatAsync(chat, user);

            _logger.LogInformation("user - {user} removed from chat with id {id}", JsonSerializer.Serialize(user), chat.Id);

            return new Unit();
        }
    }
}
