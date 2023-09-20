namespace IdentityService.BLL.DTOs.IdentityDTOs
{
    public class AuthenticatedResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}
