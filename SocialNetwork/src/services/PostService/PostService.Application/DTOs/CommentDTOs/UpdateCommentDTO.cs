namespace PostService.Application.DTOs.CommentDTOs
{
    public class UpdateCommentDTO
    {
        public Guid Id { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
