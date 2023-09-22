using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand
{
    public class UpdateChatMessageCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid MessageId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public UpdateChatMessageCommand(UpdateChatMessageDTO updateChatMessageDTO, Guid authenticatedUserId)
        {
            ChatId = updateChatMessageDTO.ChatId;
            Text = updateChatMessageDTO.Text;
            MessageId = updateChatMessageDTO.MessageId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
