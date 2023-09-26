using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand
{
    internal class RemoveChatMessageFromUserCommandHandler : IRequestHandler<RemoveChatMessageFromUserCommand>
    {
        private readonly IChatRepository _chatRepository;

        public RemoveChatMessageFromUserCommandHandler(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Unit> Handle(RemoveChatMessageFromUserCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            if (!chat.Messages.Any(m => m.Id == DTO.MessageId))
            {
                throw new NotFoundException($"no such message with id = {DTO.MessageId}");
            }

            if (!chat.Users.Any(u => u.Id == DTO.UserId))
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            await _chatRepository.RemoveChatMessageFromUserAsync(DTO.ChatId, DTO.MessageId, DTO.UserId);

            return new Unit();
        }
    }
}
