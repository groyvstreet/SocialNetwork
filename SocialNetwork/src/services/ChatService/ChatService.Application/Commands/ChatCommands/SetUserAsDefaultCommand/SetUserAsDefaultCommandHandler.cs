using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand
{
    public class SetUserAsDefaultCommandHandler : IRequestHandler<SetUserAsDefaultCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public SetUserAsDefaultCommandHandler(IChatRepository chatRepository,
                                              IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(SetUserAsDefaultCommand request, CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == request.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {request.ChatId}");
            }

            // check user

            await _chatRepository.SetUserAsChatAdminAsync(request.ChatId, request.UserId, false);

            return new Unit();
        }
    }
}
