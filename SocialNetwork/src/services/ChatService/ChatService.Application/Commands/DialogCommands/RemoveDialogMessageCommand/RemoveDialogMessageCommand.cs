using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageCommand
{
    public class RemoveDialogMessageCommand : IRequest
    {
        public RemoveDialogMessageDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveDialogMessageCommand(RemoveDialogMessageDTO removeDialogMessageDTO, Guid authenticatedUserId)
        {
            DTO = removeDialogMessageDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
