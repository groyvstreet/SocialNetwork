using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand
{
    public class RemoveDialogMessageFromUserCommand : IRequest
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }
    }
}
