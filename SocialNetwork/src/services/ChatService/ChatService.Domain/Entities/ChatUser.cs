namespace ChatService.Domain.Entities
{
    public class ChatUser : User
    {
        public bool IsAdmin { get; set; }

        public List<string> InvitedUsers { get; set; }
    }
}
