using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatMessageCommand
{
    public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public AddChatMessageCommandHandler(IChatRepository chatRepository,
                                            IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
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

            var message = new Message
            {
                DateTime = DateTimeOffset.Now,
                Text = request.Text,
                User = user
            };
            await _chatRepository.AddChatMessageAsync(request.ChatId, message);
            await _chatRepository.UpdateFieldAsync(chat, c => c.MessageCount, chat.MessageCount + 1);

            return new Unit();
        }
    }
}
