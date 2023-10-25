namespace ChatService.Application.DTOs.MessageDTOs
{
    public class AddDialogMessageDTO
    {
        public string Text { get; set; } = string.Empty;

        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        public DateTimeOffset? DateTime { get; set; }
      
        public Guid? PostId { get; set; }
    }
}
