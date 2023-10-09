using System.Security.Claims;

namespace PostService.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid AuthenticatedUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            _ = id ?? throw new NullReferenceException(nameof(id));

            return Guid.Parse(id);
        }
    }
}
