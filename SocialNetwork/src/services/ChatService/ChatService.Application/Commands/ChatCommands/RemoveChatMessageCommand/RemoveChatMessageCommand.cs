using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.RemoveChatMessageCommand
{
    public class RemoveChatMessageCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public RemoveChatMessageCommand(RemoveChatMessageDTO removeChatMessageDTO, Guid authenticatedUserId)
        {
            ChatId = removeChatMessageDTO.ChatId;
            MessageId = removeChatMessageDTO.MessageId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
