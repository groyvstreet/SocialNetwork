using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand
{
    public class UpdateChatMessageCommandHandler : IRequestHandler<UpdateChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public UpdateChatMessageCommandHandler(IChatRepository chatRepository,
                                               IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(UpdateChatMessageCommand request, CancellationToken cancellationToken)
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

            await _chatRepository.UpdateChatMessageAsync(request.ChatId, request.MessageId, request.Text);
            
            return new Unit();
        }
    }
}
