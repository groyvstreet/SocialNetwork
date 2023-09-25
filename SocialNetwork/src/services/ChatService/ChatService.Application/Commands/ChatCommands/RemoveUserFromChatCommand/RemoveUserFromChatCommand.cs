using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand
{
    public class RemoveUserFromChatCommand : IRequest
    {
        public RemoveUserFromChatDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveUserFromChatCommand(RemoveUserFromChatDTO removeUserFromChatDTO, Guid authenticatedUserId)
        {
            DTO = removeUserFromChatDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
