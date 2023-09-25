namespace ChatService.Domain.Entities
{
    public class Message : Entity
    {
        public DateTimeOffset DateTime { get; set; }

        public string Text { get; set; } = string.Empty;

        public User User { get; set; }

        public List<string> UsersRemoved { get; set; } = new List<string>();
    }
}
