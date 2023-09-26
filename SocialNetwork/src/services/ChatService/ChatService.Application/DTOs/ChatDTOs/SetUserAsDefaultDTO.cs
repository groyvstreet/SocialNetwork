namespace ChatService.Application.DTOs.ChatDTOs
{
    public class SetUserAsDefaultDTO
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }
    }
}
