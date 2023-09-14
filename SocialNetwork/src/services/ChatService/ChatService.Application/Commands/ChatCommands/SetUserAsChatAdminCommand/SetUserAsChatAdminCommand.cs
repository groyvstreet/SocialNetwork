using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand
{
    public class SetUserAsChatAdminCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }
    }
}
