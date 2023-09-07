namespace IdentityService.DAL.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpirationTime { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }
    }
}
