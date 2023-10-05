namespace ChatService.Domain.Entities
{
    public class ChatUser : User
    {
        public bool IsAdmin { get; set; } = true;

        public List<string> InvitedUsers { get; set; } = new List<string>();
    }
}
