using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddUserToChatCommand
{
    public class AddUserToChatCommand : IRequest
    {
        public AddUserToChatDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public AddUserToChatCommand(AddUserToChatDTO addUserToChatDTO, Guid authenticatedUserId)
        {
            DTO = addUserToChatDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
