using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand
{
    public class RemoveUserFromChatCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }
    }
}
