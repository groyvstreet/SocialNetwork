namespace PostService.Application.DTOs.PostDTOs
{
    public class GetPostDTO
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public string Text { get; set; } = string.Empty;

        public ulong CommentCount { get; set; }

        public ulong LikeCount { get; set; }

        public Guid UserProfileId { get; set; }
    }
}
