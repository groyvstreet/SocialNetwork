using ChatService.Domain.Entities;

namespace ChatService.Application.DTOs.MessageDTOs
{
    public class GetMessageDTO
    {
        public Guid Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string Text { get; set; } = string.Empty;

        public string? PostId { get; set; }

        public User User { get; set; }
    }
}
