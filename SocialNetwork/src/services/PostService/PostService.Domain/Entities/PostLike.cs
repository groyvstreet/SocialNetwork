namespace PostService.Domain.Entities
{
    public class PostLike
    {
        public Guid Id { get; set; }

        public Post Post { get; set; }

        public Guid PostId { get; set; }

        public User UserProfile { get; set; }

        public Guid UserProfileId { get; set; }
    }
}
