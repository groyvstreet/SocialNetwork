using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.RemoveDialogMessageFromUserCommand
{
    public class RemoveDialogMessageFromUserCommand : IRequest
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveDialogMessageFromUserCommand(RemoveDialogMessageFromUserDTO removeDialogMessageFromUserDTO, Guid authenticatedUserId)
        {
            DialogId = removeDialogMessageFromUserDTO.DialogId;
            MessageId = removeDialogMessageFromUserDTO.MessageId;
            UserId = removeDialogMessageFromUserDTO.UserId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
