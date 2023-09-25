using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatMessageCommand
{
    public class AddChatMessageCommand : IRequest
    {
        public AddChatMessageDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public AddChatMessageCommand(AddChatMessageDTO addChatMessageDTO, Guid authenticatedUserId)
        {
            DTO = addChatMessageDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
