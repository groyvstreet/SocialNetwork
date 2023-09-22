using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageCommand
{
    public class RemoveChatMessageCommandHandler : IRequestHandler<RemoveChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public RemoveChatMessageCommandHandler(IChatRepository chatRepository,
                                               IUserRepository userRepository,
                                               IHubContext<ChatHub, IChatHub> hubContext)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(RemoveChatMessageCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            var message = chat.Messages.FirstOrDefault(m => m.Id == request.MessageId);

            if (message is null)
            {
                throw new NotFoundException($"no such message with id = {request.MessageId}");
            }

            if (message.User.Id != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            await _chatRepository.RemoveChatMessageAsync(request.ChatId, request.MessageId);
            await _chatRepository.UpdateFieldAsync(chat, c => c.MessageCount, chat.MessageCount - 1);

            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users = new List<ChatUser>();
            chat.Messages = new List<Message> { chat.Messages.First(m => m.Id == request.MessageId) };
            chat.MessageCount--;
            await _hubContext.Clients.Users(userIds).RemoveMessage(chat);

            return new Unit();
        }
    }
}
