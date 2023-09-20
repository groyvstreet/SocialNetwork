namespace PostService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public string Image { get; set; } = string.Empty;

        public List<Post> Posts { get; set; }

        public List<Comment> Comments { get; set; }

        public List<PostLike> PostLikes { get; set; }

        public List<CommentLike> CommentLikes { get; set; }
    }
}
