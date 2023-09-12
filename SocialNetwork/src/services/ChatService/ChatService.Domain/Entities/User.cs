namespace ChatService.Domain.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;
    }
}
