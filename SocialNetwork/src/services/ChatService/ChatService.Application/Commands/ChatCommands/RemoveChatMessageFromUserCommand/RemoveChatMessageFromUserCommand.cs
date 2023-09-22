using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageFromUserCommand
{
    public class RemoveChatMessageFromUserCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveChatMessageFromUserCommand(RemoveChatMessageFromUserDTO removeChatMessageFromUserDTO, Guid authenticatedUserId)
        {
            ChatId = removeChatMessageFromUserDTO.ChatId;
            MessageId = removeChatMessageFromUserDTO.MessageId;
            UserId = removeChatMessageFromUserDTO.UserId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
