using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand
{
    public class SetUserAsChatAdminCommandHandler : IRequestHandler<SetUserAsChatAdminCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public SetUserAsChatAdminCommandHandler(IChatRepository chatRepository,
                                                IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(SetUserAsChatAdminCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            // check user

            await _chatRepository.SetUserAsChatAdminAsync(request.ChatId, request.UserId, true);

            return new Unit();
        }
    }
}
