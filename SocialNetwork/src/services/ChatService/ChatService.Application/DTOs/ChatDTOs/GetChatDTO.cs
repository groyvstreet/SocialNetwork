using ChatService.Application.DTOs.MessageDTOs;
using ChatService.Domain.Entities;

namespace ChatService.Application.DTOs.ChatDTOs
{
    public class GetChatDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ulong UserCount { get; set; }

        public ulong MessageCount { get; set; }

        public List<ChatUser> Users { get; set; }

        public List<GetMessageDTO> Messages { get; set; }
    }
}
