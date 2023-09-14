using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatCommand
{
    public class AddChatCommand : IRequest
    {
        public string Name { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
