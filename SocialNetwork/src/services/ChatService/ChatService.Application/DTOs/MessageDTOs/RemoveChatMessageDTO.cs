namespace ChatService.Application.DTOs.MessageDTOs
{
    public class RemoveChatMessageDTO
    {
        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }
    }
}
