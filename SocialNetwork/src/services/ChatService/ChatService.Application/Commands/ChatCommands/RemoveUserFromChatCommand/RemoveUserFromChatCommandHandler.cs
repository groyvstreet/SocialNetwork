using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand
{
    public class RemoveUserFromChatCommandHandler : IRequestHandler<RemoveUserFromChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public RemoveUserFromChatCommandHandler(IChatRepository chatRepository,
                                                IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(RemoveUserFromChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            // check user

            await _chatRepository.RemoveUserFromChatAsync(request.ChatId, request.UserId);
            await _chatRepository.UpdateFieldAsync(chat, c => c.UserCount, chat.UserCount - 1);

            return new Unit();
        }
    }
}
