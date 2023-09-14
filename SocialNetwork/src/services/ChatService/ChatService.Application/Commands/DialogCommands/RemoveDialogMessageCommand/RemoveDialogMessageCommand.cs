using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand
{
    public class RemoveDialogMessageCommand : IRequest
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }
    }
}
