namespace PostService.Application.DTOs.CommentDTOs
{
    public class AddCommentDTO
    {
        public string Text { get; set; } = string.Empty;

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }
    }
}
