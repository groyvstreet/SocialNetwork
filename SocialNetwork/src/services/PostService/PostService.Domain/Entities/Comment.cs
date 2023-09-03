namespace PostService.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string Text { get; set; } = string.Empty;

        public ulong LikeCount { get; set; }

        public Post Post { get; set; }

        public Guid PostId { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }
    }
}
