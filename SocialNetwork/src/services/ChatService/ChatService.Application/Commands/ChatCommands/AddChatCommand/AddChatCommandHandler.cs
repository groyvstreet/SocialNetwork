using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatCommand
{
    public class AddChatCommandHandler : IRequestHandler<AddChatCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public AddChatCommandHandler(IChatRepository chatRepository,
                                     IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(AddChatCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            var chat = new Chat
            {
                Name = request.Name,
                UserCount = 1,
                Users = new List<ChatUser>
                {
                    new ChatUser
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Image = user.Image,
                        IsAdmin = true
                    }
                }
            };
            await _chatRepository.AddAsync(chat);

            return new Unit();
        }
    }
}
