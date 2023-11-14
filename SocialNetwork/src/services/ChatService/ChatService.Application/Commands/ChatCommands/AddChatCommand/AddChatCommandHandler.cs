using AutoMapper;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.AddChatCommand
{
    public class AddChatCommandHandler : IRequestHandler<AddChatCommand>
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ILogger<AddChatCommandHandler> _logger;
        private readonly ICacheRepository<User> _userCacheRepository;

        public AddChatCommandHandler(IMapper mapper,
                                     IChatRepository chatRepository,
                                     IUserRepository userRepository,
                                     IChatNotificationService chatNotificationService,
                                     ILogger<AddChatCommandHandler> logger,
                                     ICacheRepository<User> userCacheRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _chatNotificationService = chatNotificationService;
            _logger = logger;
            _userCacheRepository = userCacheRepository;
        }

        public async Task<Unit> Handle(AddChatCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
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

            var chatUser = _mapper.Map<ChatUser>(user);
            chatUser.IsAdmin = true;

            var chat = new Chat
            {
                Name = DTO.Name,
                UserCount = 1,
                Users = new List<ChatUser> { chatUser }
            };
            await _chatRepository.AddAsync(chat);

            await _chatNotificationService.CreateChatAsync(chat);

            _logger.LogInformation("chat - {chat} added", JsonSerializer.Serialize(chat));

            return new Unit();
        }
    }
}
