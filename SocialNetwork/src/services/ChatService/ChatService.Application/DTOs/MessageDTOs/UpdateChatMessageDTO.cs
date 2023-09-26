namespace ChatService.Application.DTOs.MessageDTOs
{
    public class UpdateChatMessageDTO
    {
        public Guid ChatId { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid MessageId { get; set; }
    }
}
