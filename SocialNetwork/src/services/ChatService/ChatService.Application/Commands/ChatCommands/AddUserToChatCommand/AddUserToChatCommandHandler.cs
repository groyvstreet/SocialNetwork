using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddUserToChatCommand
{
    internal class AddUserToChatCommandHandler : IRequestHandler<AddUserToChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public AddUserToChatCommandHandler(IChatRepository chatRepository,
                                           IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
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

            var chatUser = new ChatUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Image = user.Image,
                IsAdmin = false
            };

            await _chatRepository.AddUserToChatAsync(request.ChatId, chatUser);
            await _chatRepository.AddUserToInvitedUsers(request.ChatId, request.UserId, request.InvitedUserId);
            await _chatRepository.UpdateFieldAsync(chat, c => c.UserCount, chat.UserCount + 1);

            return new Unit();
        }
    }
}
