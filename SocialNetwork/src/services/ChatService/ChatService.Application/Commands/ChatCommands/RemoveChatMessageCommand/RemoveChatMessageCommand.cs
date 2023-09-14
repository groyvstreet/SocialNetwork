using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageCommand
{
    public class RemoveChatMessageCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }
    }
}
