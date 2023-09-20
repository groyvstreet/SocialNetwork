namespace PostService.Application.DTOs.CommentDTOs
{
    public class GetCommentDTO
    {
        public Guid Id { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string Text { get; set; } = string.Empty;

        public ulong LikeCount { get; set; }

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }
    }
}
