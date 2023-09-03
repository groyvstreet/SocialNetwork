namespace PostService.Domain.Entities
{
    public class PostLike
    {
        public Guid Id { get; set; }

        public Post Post { get; set; }

        public Guid PostId { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }
    }
}
