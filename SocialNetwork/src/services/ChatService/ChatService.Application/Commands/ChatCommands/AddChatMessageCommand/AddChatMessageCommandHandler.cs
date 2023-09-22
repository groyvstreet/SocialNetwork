using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.ChatCommands.AddChatMessageCommand
{
    public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public AddChatMessageCommandHandler(IChatRepository chatRepository,
                                            IUserRepository userRepository,
                                            IHubContext<ChatHub, IChatHub> hubContext)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            if (!chat.Users.Any(u => u.Id == request.UserId))
            {
                throw new ForbiddenException($"no such user with id = {request.UserId} in chat with id = {request.ChatId}");
            }

            var message = new Message
            {
                DateTime = DateTimeOffset.Now,
                Text = request.Text,
                User = user
            };
            await _chatRepository.AddChatMessageAsync(request.ChatId, message);
            await _chatRepository.UpdateFieldAsync(chat, c => c.MessageCount, chat.MessageCount + 1);

            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users = new List<ChatUser>();
            chat.Messages = new List<Message> { message };
            await _hubContext.Clients.Users(userIds).SendMessage(chat);

            return new Unit();
        }
    }
}
