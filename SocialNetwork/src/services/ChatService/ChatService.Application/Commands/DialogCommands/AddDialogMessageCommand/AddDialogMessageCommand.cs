using MediatR;

namespace ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand
{
    public class AddDialogMessageCommand : IRequest
    {
        public string Text { get; set; } = string.Empty;

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }
    }
}
