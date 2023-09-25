namespace ChatService.Domain.Entities
{
    public class Dialog : Entity
    {
        public long MessageCount { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();

        public List<User> Users { get; set; } = new List<User>();
    }
}
