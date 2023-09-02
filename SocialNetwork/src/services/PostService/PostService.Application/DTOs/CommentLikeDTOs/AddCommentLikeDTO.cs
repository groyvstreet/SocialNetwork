namespace PostService.Application.DTOs.CommentLikeDTOs
{
    public class AddCommentLikeDTO
    {
        public Guid CommentId { get; set; }

        public Guid UserProfileId { get; set; }
    }
}
