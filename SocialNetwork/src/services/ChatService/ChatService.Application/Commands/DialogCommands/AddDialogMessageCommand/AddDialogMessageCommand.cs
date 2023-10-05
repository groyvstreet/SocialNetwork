using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand
{
    public class AddDialogMessageCommand : IRequest
    {
        public AddDialogMessageDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public DateTimeOffset? DateTime { get; set; }

        public AddDialogMessageCommand(AddDialogMessageDTO addDialogMessageDTO, Guid authenticatedUserId, DateTimeOffset? dateTime)
        {
            DTO = addDialogMessageDTO;
            AuthenticatedUserId = authenticatedUserId;
            DateTime = dateTime;
        }
    }
}
