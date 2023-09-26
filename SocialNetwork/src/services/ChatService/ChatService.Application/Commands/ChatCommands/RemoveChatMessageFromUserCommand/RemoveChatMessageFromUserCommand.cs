using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand
{
    public class RemoveChatMessageFromUserCommand : IRequest
    {
        public RemoveChatMessageFromUserDTO DTO { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveChatMessageFromUserCommand(RemoveChatMessageFromUserDTO removeChatMessageFromUserDTO, Guid authenticatedUserId)
        {
            DTO = removeChatMessageFromUserDTO;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
