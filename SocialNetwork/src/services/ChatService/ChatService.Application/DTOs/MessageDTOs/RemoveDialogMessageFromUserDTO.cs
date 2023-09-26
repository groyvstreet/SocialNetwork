namespace ChatService.Application.DTOs.MessageDTOs
{
    public class RemoveDialogMessageFromUserDTO
    {
        public Guid DialogId { get; set; }

        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }
    }
}
