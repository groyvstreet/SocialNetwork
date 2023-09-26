namespace ChatService.Application.DTOs.ChatDTOs
{
    public class AddChatDTO
    {
        public string Name { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
