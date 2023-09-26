namespace ChatService.Application.DTOs.ChatDTOs
{
    public class SetUserAsChatAdminDTO
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }
    }
}
