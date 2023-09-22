using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatCommand
{
    public class UpdateChatCommand : IRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid AuthenticatedUserId { get; set; }

        public UpdateChatCommand(UpdateChatDTO updateChatDTO, Guid authenticatedUserId)
        {
            Id = updateChatDTO.Id;
            Name = updateChatDTO.Name;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
