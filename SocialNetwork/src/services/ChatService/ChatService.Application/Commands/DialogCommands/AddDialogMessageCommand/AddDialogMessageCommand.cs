using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand
{
    public class AddDialogMessageCommand : IRequest
    {
        public string Text { get; set; } = string.Empty;

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public AddDialogMessageCommand(AddDialogMessageDTO addDialogMessageDTO, Guid authenticatedUserId)
        {
            Text = addDialogMessageDTO.Text;
            SenderId = addDialogMessageDTO.SenderId;
            ReceiverId = addDialogMessageDTO.ReceiverId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
