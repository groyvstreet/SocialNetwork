namespace ChatService.Domain.Entities
{
    public class Chat : Entity
    {
        public string Name { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public long UserCount { get; set; }

        public long MessageCount { get; set; }

        public List<ChatUser> Users { get; set; } = new List<ChatUser>();

        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
