using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand
{
    public class UpdateChatMessageCommand : IRequest
    {
        public UpdateChatMessageDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public UpdateChatMessageCommand(UpdateChatMessageDTO updateChatMessageDTO, Guid authenticatedUserId)
        {
            DTO = updateChatMessageDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
