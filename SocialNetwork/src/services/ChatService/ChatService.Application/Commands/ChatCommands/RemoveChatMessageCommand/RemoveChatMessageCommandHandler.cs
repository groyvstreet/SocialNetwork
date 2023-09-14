using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageCommand
{
    public class RemoveChatMessageCommandHandler : IRequestHandler<RemoveChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public RemoveChatMessageCommandHandler(IChatRepository chatRepository,
                                               IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(RemoveChatMessageCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            /*var message = await _chatRepository.GetChatMessageAsync(request.ChatId, request.MessageId);

            if (message is null)
            {
                throw new NotFoundException($"no such message with id = {request.MessageId}");
            }*/

            await _chatRepository.RemoveChatMessageAsync(request.ChatId, request.MessageId);
            await _chatRepository.UpdateFieldAsync(chat, c => c.MessageCount, chat.MessageCount - 1);

            return new Unit();
        }
    }
}
