namespace ChatService.Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ulong UserCount { get; set; }

        public ulong MessageCount { get; set; }

        public List<ChatUser> Users { get; set; }

        public List<Message> Messages { get; set; }
    }
}
