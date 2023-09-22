namespace ChatService.Application.DTOs.MessageDTOs
{
    public class RemoveDialogMessageDTO
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }
    }
}
