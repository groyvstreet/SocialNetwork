using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.SetUserAsChatAdminCommand
{
    public class SetUserAsChatAdminCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public SetUserAsChatAdminCommand(SetUserAsChatAdminDTO setUserAsChatAdminDTO, Guid authenticatedUserId)
        {
            ChatId = setUserAsChatAdminDTO.ChatId;
            UserId = setUserAsChatAdminDTO.UserId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
