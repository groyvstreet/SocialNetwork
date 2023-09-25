using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatCommand
{
    public class AddChatCommand : IRequest
    {
        public AddChatDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public AddChatCommand(AddChatDTO addChatDTO, Guid authenticatedUserId)
        {
            DTO = addChatDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
