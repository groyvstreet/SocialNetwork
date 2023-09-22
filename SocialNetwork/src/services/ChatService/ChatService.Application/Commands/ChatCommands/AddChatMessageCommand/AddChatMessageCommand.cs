using ChatService.Application.DTOs.MessageDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatMessageCommand
{
    public class AddChatMessageCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public AddChatMessageCommand(AddChatMessageDTO addChatMessageDTO, Guid authenticatedUserId)
        {
            ChatId = addChatMessageDTO.ChatId;
            Text = addChatMessageDTO.Text;
            UserId = addChatMessageDTO.UserId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
