using System.Text.Json.Serialization;

namespace PostService.Domain.Entities
{
    public class PostLike
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Post Post { get; set; }

        public Guid PostId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public Guid UserId { get; set; }
    }
}
