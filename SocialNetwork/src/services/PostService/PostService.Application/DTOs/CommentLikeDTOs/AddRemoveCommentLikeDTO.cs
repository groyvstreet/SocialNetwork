namespace PostService.Application.DTOs.CommentLikeDTOs
{
    public class AddRemoveCommentLikeDTO
    {
        public Guid CommentId { get; set; }

        public Guid UserId { get; set; }
    }
}
