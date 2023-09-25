using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand
{
    public class RemoveDialogMessageFromUserCommand : IRequest
    {
        public RemoveDialogMessageFromUserDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveDialogMessageFromUserCommand(RemoveDialogMessageFromUserDTO removeDialogMessageFromUserDTO, Guid authenticatedUserId)
        {
            DTO = removeDialogMessageFromUserDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
