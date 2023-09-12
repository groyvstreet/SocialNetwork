namespace ChatService.Domain.Entities
{
    public class Message : IEntity
    {
        public Guid Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string Text { get; set; } = string.Empty;

        public User User { get; set; }

        public List<string> UsersRemoved { get; set; }
    }
}
