namespace ChatService.Application.DTOs.MessageDTOs
{
    public class AddChatMessageDTO
    {
        public Guid ChatId { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public DateTimeOffset? DateTime { get; set; }

        public Guid? PostId { get; set; }
    }
}
