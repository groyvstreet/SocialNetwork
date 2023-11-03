﻿using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand
{
    public class SetUserAsChatAdminCommandHandler : IRequestHandler<SetUserAsChatAdminCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ILogger<SetUserAsChatAdminCommandHandler> _logger;

        public SetUserAsChatAdminCommandHandler(IChatRepository chatRepository,
                                                IChatNotificationService chatNotificationService,
                                                ILogger<SetUserAsChatAdminCommandHandler> logger)
        {
            _chatRepository = chatRepository;
            _chatNotificationService = chatNotificationService;
            _logger = logger;
        }

        public async Task<Unit> Handle(SetUserAsChatAdminCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(chat => chat.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            var authenticatedUser = chat.Users.FirstOrDefault(user => user.Id == request.AuthenticatedUserId);

            if (authenticatedUser is null || !authenticatedUser.IsAdmin)
            {
                throw new ForbiddenException("forbidden");
            }

            var user = chat.Users.First(user => user.Id == DTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            await _chatRepository.SetUserAsChatAdminAsync(DTO.ChatId, DTO.UserId, true);

            await _chatNotificationService.SetUserAsChatAdminAsync(chat, user, true);

            _logger.LogInformation("user - {user} setted as admin in chat with id {id}", JsonSerializer.Serialize(user), chat.Id);

            return new Unit();
        }
    }
}
