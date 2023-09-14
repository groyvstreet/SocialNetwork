namespace ChatService.Domain.Entities
{
    public class Chat : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public ulong UserCount { get; set; }

        public ulong MessageCount { get; set; }

        public List<ChatUser> Users { get; set; } = new List<ChatUser>();

        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
