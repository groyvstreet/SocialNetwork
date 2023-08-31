namespace PostService.Domain.Entities
{
    public class PostsUserProfile
    {
        public Guid Id { get; set; }

        public Post Post { get; set; }

        public Guid PostId { get; set; }

        public UserProfile UserProfile { get; set; }

        public Guid UserProfileId { get; set; }
    }
}
