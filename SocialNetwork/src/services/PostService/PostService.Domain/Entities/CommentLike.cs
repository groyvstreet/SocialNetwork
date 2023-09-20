namespace PostService.Domain.Entities
{
    public class CommentLike
    {
        public Guid Id { get; set; }

        public Comment Comment { get; set; }

        public Guid CommentId { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }
    }
}
