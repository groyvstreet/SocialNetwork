namespace PostService.Application.DTOs.UserProfileDTOs
{
    public class GetUserDTO
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public string Image { get; set; } = string.Empty;
    }
}
