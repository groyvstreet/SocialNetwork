using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand
{
    public class UpdateDialogMessageCommand : IRequest
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid AuthenticatedUserId { get; set; }

        public UpdateDialogMessageCommand(UpdateDialogMessageDTO updateDialogMessageDTO, Guid authenticatedUserId)
        {
            DialogId = updateDialogMessageDTO.DialogId;
            MessageId = updateDialogMessageDTO.MessageId;
            Text = updateDialogMessageDTO.Text;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
