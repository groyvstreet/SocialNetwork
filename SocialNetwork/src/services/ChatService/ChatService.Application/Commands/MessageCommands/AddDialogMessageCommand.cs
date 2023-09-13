using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.MessageCommands
{
    public class AddDialogMessageCommand : IRequest<Message>
    {
        public string Text { get; set; } = string.Empty;

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }
    }
}
