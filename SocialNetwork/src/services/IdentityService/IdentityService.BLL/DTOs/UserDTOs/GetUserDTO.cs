namespace IdentityService.BLL.DTOs.UserDTOs
{
    public class GetUserDTO
    {
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }
    }
}
