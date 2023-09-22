using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.ChatCommands.AddChatCommand
{
    public class AddChatCommandHandler : IRequestHandler<AddChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public AddChatCommandHandler(IChatRepository chatRepository,
                                     IUserRepository userRepository,
                                     IHubContext<ChatHub, IChatHub> hubContext)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(AddChatCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            var chat = new Chat
            {
                Name = request.Name,
                UserCount = 1,
                Users = new List<ChatUser>
                {
                    new ChatUser
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Image = user.Image,
                        IsAdmin = true
                    }
                }
            };
            await _chatRepository.AddAsync(chat);

            await _hubContext.Clients.User(user.Id.ToString()).CreateChat(chat);

            return new Unit();
        }
    }
}
