namespace PostService.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string Text { get; set; } = string.Empty;

        public ulong CommentCount { get; set; }

        public ulong LikeCount { get; set; }

        public UserProfile UserProfile { get; set; }

        public Guid UserProfileId { get; set; }
    }
}
