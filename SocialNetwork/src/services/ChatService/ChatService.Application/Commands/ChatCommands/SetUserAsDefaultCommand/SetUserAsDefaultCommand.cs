using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand
{
    public class SetUserAsDefaultCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }
    }
}
