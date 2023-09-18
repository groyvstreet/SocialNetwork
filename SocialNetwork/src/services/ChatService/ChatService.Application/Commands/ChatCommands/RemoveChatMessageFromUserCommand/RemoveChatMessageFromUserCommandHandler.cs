using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand
{
    internal class RemoveChatMessageFromUserCommandHandler : IRequestHandler<RemoveChatMessageFromUserCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public RemoveChatMessageFromUserCommandHandler(IChatRepository chatRepository,
                                                       IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(RemoveChatMessageFromUserCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            if (!chat.Messages.Any(m => m.Id == request.MessageId))
            {
                throw new NotFoundException($"no such message with id = {request.MessageId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            await _chatRepository.RemoveChatMessageFromUserAsync(request.ChatId, request.MessageId, request.UserId);

            return new Unit();
        }
    }
}
