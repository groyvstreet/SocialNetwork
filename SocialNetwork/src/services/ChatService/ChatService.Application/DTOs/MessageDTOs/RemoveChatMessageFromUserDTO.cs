namespace ChatService.Application.DTOs.MessageDTOs
{
    public class RemoveChatMessageFromUserDTO
    {
        public Guid ChatId { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }
    }
}
