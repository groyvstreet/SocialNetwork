using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand
{
    public class RemoveUserFromChatCommandHandler : IRequestHandler<RemoveUserFromChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public RemoveUserFromChatCommandHandler(IChatRepository chatRepository,
                                                IUserRepository userRepository,
                                                IHubContext<ChatHub, IChatHub> hubContext)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(RemoveUserFromChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            if (!chat.Users.Any(u => u.Id == request.UserId))
            {
                throw new NotFoundException($"no such user with id = {request.UserId} in chat with id = {request.ChatId}");
            }

            await _chatRepository.RemoveUserFromChatAsync(request.ChatId, request.UserId);
            await _chatRepository.UpdateFieldAsync(chat, c => c.UserCount, chat.UserCount - 1);

            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users = new List<ChatUser> { chat.Users.First(u => u.Id == request.UserId) };
            chat.UserCount--;
            chat.Messages = new List<Message>();
            await _hubContext.Clients.Users(userIds).RemoveUserFromChat(chat);

            return new Unit();
        }
    }
}
