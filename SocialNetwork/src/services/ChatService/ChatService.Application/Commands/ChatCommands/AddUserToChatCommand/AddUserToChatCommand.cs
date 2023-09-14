using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddUserToChatCommand
{
    public class AddUserToChatCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public Guid InvitedUserId { get; set; }
    }
}
