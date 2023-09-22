using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsDefaultCommand
{
    public class SetUserAsDefaultCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public SetUserAsDefaultCommand(SetUserAsDefaultDTO setUserAsDefaultDTO, Guid authenticatedUserId)
        {
            ChatId = setUserAsDefaultDTO.ChatId;
            UserId = setUserAsDefaultDTO.UserId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
