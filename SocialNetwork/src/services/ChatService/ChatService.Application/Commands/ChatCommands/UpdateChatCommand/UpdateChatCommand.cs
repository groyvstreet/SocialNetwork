using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatCommand
{
    public class UpdateChatCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
