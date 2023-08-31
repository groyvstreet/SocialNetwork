namespace PostService.Application.DTOs.PostDTOs
{
    public class UpdatePostDTO
    {
        public Guid Id { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
