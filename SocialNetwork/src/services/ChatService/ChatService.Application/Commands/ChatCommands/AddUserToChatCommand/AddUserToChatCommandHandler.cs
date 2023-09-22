using ChatService.Application.Exceptions;
using ChatService.Application.Hubs;
using ChatService.Application.Interfaces.Hubs;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Application.Commands.ChatCommands.AddUserToChatCommand
{
    internal class AddUserToChatCommandHandler : IRequestHandler<AddUserToChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public AddUserToChatCommandHandler(IChatRepository chatRepository,
                                           IUserRepository userRepository,
                                           IHubContext<ChatHub, IChatHub> hubContext)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<Unit> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

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

            var invitedUser = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.InvitedUserId);

            if (invitedUser is null)
            {
                throw new NotFoundException($"no such user with id = {request.InvitedUserId}");
            }

            var chatUser = new ChatUser
            {
                Id = invitedUser.Id,
                FirstName = invitedUser.FirstName,
                LastName = invitedUser.LastName,
                Image = invitedUser.Image,
                IsAdmin = false
            };

            await _chatRepository.AddUserToChatAsync(request.ChatId, chatUser);
            await _chatRepository.AddUserToInvitedUsers(request.ChatId, request.UserId, request.InvitedUserId);
            await _chatRepository.UpdateFieldAsync(chat, c => c.UserCount, chat.UserCount + 1);

            var userIds = chat.Users.Select(u => u.Id.ToString()).ToList();
            chat.Users = new List<ChatUser> { chatUser };
            chat.UserCount++;
            chat.Messages = new List<Message>();
            await _hubContext.Clients.Users(userIds).AddUserToChat(chat);

            return new Unit();
        }
    }
}
