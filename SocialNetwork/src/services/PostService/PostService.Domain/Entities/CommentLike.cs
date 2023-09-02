namespace PostService.Domain.Entities
{
    public class CommentLike
    {
        public Guid Id { get; set; }

        public Comment Comment { get; set; }

        public Guid CommentId { get; set; }

        public UserProfile UserProfile { get; set; }

        public Guid UserProfileId { get; set; }
    }
}
