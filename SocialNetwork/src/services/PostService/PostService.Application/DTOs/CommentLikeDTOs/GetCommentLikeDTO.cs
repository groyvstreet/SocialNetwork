namespace PostService.Application.DTOs.CommentLikeDTOs
{
    public class GetCommentLikeDTO
    {
        public Guid Id { get; set; }

        public Guid CommentId { get; set; }

        public Guid UserId { get; set; }
    }
}
