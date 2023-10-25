using System.Text.Json.Serialization;

namespace PostService.Domain.Entities
{
    public class CommentLike
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Comment Comment { get; set; }

        public Guid CommentId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public Guid UserId { get; set; }
    }
}
