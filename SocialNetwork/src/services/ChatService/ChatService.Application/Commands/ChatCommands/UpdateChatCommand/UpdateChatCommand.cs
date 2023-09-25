using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatCommand
{
    public class UpdateChatCommand : IRequest
    {
        public UpdateChatDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public UpdateChatCommand(UpdateChatDTO updateChatDTO, Guid authenticatedUserId)
        {
            DTO = updateChatDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
