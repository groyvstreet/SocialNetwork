using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand
{
    public class SetUserAsDefaultCommand : IRequest
    {
        public SetUserAsDefaultDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public SetUserAsDefaultCommand(SetUserAsDefaultDTO setUserAsDefaultDTO, Guid authenticatedUserId)
        {
            DTO = setUserAsDefaultDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
