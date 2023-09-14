using MediatR;

namespace ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand
{
    public class UpdateDialogMessageCommand : IRequest
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
