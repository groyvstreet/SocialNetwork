namespace ChatService.Application.DTOs.ChatDTOs
{
    public class AddUserToChatDTO
    {
        public Guid ChatId { get; set; }

        public Guid UserId { get; set; }

        public Guid InvitedUserId { get; set; }
    }
}
