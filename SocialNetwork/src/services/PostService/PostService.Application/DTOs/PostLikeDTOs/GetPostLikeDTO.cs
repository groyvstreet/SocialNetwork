namespace PostService.Application.DTOs.PostLikeDTOs
{
    public class GetPostLikeDTO
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }
    }
}
