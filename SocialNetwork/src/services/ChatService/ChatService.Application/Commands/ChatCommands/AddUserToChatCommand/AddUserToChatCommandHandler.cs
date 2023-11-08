using AutoMapper;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.AddUserToChatCommand
{
    internal class AddUserToChatCommandHandler : IRequestHandler<AddUserToChatCommand>
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ILogger<AddUserToChatCommandHandler> _logger;
        private readonly ICacheRepository<User> _userCacheRepository;

        public AddUserToChatCommandHandler(IMapper mapper,
                                           IChatRepository chatRepository,
                                           IUserRepository userRepository,
                                           IChatNotificationService chatNotificationService,
                                           ILogger<AddUserToChatCommandHandler> logger,
                                           ICacheRepository<User> userCacheRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _chatNotificationService = chatNotificationService;
            _logger = logger;
            _userCacheRepository = userCacheRepository;
        }

        public async Task<Unit> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
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

            var user = await _userCacheRepository.GetAsync(DTO.UserId.ToString());

            if (user is null)
            {
                user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == DTO.UserId);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {DTO.UserId}");
                }

                await _userCacheRepository.SetAsync(user.Id.ToString(), user);
            }

            if (!chat.Users.Any(user => user.Id == DTO.UserId))
            {
                throw new ForbiddenException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            var invitedUser = await _userCacheRepository.GetAsync(DTO.InvitedUserId.ToString());

            if (invitedUser is null)
            {
                invitedUser = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.InvitedUserId);

                if (invitedUser is null)
                {
                    throw new NotFoundException($"no such user with id = {DTO.InvitedUserId}");
                }

                await _userCacheRepository.SetAsync(invitedUser.Id.ToString(), invitedUser);
            }

            var chatUser = _mapper.Map<ChatUser>(invitedUser);

            await _chatRepository.AddUserToChatAsync(DTO.ChatId, chatUser);
            await _chatRepository.AddUserToInvitedUsers(DTO.ChatId, DTO.UserId, DTO.InvitedUserId);

            await _chatNotificationService.AddUsetToChatAsync(chat, chatUser);

            _logger.LogInformation("user - {user} added to chat with id {id}", JsonSerializer.Serialize(invitedUser), chat.Id);

            return new Unit();
        }
    }
}
