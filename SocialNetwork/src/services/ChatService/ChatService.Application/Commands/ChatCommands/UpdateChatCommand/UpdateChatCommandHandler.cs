using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatCommand
{
    public class UpdateChatCommandHandler : IRequestHandler<UpdateChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public UpdateChatCommandHandler(IChatRepository chatRepository,
                                        IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(UpdateChatCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.Id);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.Id}");
            }

            await _chatRepository.UpdateFieldAsync(chat, c => c.Name, request.Name);

            return new Unit();
        }
    }
}
