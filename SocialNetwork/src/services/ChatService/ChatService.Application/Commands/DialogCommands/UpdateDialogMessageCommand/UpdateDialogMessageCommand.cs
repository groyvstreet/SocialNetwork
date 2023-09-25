using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand
{
    public class UpdateDialogMessageCommand : IRequest
    {
        public UpdateDialogMessageDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public UpdateDialogMessageCommand(UpdateDialogMessageDTO updateDialogMessageDTO, Guid authenticatedUserId)
        {
            DTO = updateDialogMessageDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
