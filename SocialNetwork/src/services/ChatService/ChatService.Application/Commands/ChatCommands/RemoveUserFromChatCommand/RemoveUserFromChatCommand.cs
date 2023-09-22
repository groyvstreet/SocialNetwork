using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveUserFromChatCommand
{
    public class RemoveUserFromChatCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveUserFromChatCommand(RemoveUserFromChatDTO removeUserFromChatDTO, Guid authenticatedUserId)
        {
            ChatId = removeUserFromChatDTO.ChatId;
            UserId = removeUserFromChatDTO.UserId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
