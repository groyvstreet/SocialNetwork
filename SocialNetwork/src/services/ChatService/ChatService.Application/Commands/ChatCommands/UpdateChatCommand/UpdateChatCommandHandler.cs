using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatCommand
{
    public class UpdateChatCommandHandler : IRequestHandler<UpdateChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public UpdateChatCommandHandler(IChatRepository chatRepository,
                                        IUserRepository userRepository,
                                        IHubContext<ChatHub, IChatHub> hubContext)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(UpdateChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.Id);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.Id}");
            }

            await _chatRepository.UpdateFieldAsync(chat, c => c.Name, request.Name);

            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Name = request.Name;
            chat.Messages.Clear();
            chat.Users.Clear();
            await _hubContext.Clients.Users(userIds).UpdateChat(chat);

            return new Unit();
        }
    }
}
