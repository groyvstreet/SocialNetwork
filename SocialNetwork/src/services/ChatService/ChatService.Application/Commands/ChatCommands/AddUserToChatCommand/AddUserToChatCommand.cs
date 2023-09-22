using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddUserToChatCommand
{
    public class AddUserToChatCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public Guid InvitedUserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public AddUserToChatCommand(AddUserToChatDTO addUserToChatDTO, Guid authenticatedUserId)
        {
            ChatId = addUserToChatDTO.ChatId;
            UserId = addUserToChatDTO.UserId;
            InvitedUserId = addUserToChatDTO.InvitedUserId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
