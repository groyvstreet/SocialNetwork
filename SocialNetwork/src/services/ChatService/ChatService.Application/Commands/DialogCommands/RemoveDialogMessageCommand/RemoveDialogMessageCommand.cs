using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand
{
    public class RemoveDialogMessageCommand : IRequest
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveDialogMessageCommand(RemoveDialogMessageDTO removeDialogMessageDTO, Guid authenticatedUserId)
        {
            DialogId = removeDialogMessageDTO.DialogId;
            MessageId = removeDialogMessageDTO.MessageId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
