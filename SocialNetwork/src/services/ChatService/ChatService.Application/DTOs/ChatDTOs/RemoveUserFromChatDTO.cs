namespace ChatService.Application.DTOs.ChatDTOs
{
    public class RemoveUserFromChatDTO
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }
    }
}
