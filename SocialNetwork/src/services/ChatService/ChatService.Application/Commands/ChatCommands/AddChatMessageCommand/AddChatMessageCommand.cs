using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatMessageCommand
{
    public class AddChatMessageCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
