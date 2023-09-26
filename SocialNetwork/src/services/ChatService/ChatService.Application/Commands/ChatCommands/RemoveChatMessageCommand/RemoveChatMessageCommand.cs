using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageCommand
{
    public class RemoveChatMessageCommand : IRequest
    {
        public RemoveChatMessageDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveChatMessageCommand(RemoveChatMessageDTO removeChatMessageDTO, Guid authenticatedUserId)
        {
            DTO = removeChatMessageDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
