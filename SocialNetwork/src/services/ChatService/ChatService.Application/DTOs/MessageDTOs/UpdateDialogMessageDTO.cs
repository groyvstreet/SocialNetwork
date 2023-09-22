namespace ChatService.Application.DTOs.MessageDTOs
{
    public class UpdateDialogMessageDTO
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
