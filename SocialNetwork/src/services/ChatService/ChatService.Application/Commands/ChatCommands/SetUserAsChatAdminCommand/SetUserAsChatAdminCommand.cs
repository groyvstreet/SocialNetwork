using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand
{
    public class SetUserAsChatAdminCommand : IRequest
    {
        public SetUserAsChatAdminDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public SetUserAsChatAdminCommand(SetUserAsChatAdminDTO setUserAsChatAdminDTO, Guid authenticatedUserId)
        {
            DTO = setUserAsChatAdminDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
