namespace ChatService.Domain.Entities
{
    public class User : Entity
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;
    }
}
