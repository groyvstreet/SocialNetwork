namespace PostService.Application.DTOs.PostLikeDTOs
{
    public class AddRemovePostLikeDTO
    {
        public Guid PostId { get; set; }

        public Guid UserId { get; set; }
    }
}
