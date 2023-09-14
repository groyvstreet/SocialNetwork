using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand
{
    public class RemoveChatMessageFromUserCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }
    }
}
