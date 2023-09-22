using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatCommand
{
    public class AddChatCommand : IRequest
    {
        public string Name { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public AddChatCommand(AddChatDTO addChatDTO)
        {
            Name = addChatDTO.Name;
            UserId = addChatDTO.UserId;
        }
    }
}
