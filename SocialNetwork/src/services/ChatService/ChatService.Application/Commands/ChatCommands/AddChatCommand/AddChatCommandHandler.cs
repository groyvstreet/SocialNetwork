using AutoMapper;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatCommand
{
    public class AddChatCommandHandler : IRequestHandler<AddChatCommand>
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatNotificationService _chatNotificationService;

        public AddChatCommandHandler(IMapper mapper,
                                     IChatRepository chatRepository,
                                     IUserRepository userRepository,
                                     IChatNotificationService chatNotificationService)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _chatNotificationService = chatNotificationService;
        }

        public async Task<Unit> Handle(AddChatCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId}");
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

            return new Unit();
        }
    }
}
