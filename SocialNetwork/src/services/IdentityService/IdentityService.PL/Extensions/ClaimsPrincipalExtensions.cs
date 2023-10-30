using System.Security.Claims;

namespace IdentityService.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string AuthenticatedUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            _ = id ?? throw new NullReferenceException(nameof(id));

            return id;
        }

        public static string AuthenticatedUserRole(this ClaimsPrincipal user)
        {
            var role = user.FindFirstValue(ClaimTypes.Role);
            _ = role ?? throw new NullReferenceException(nameof(role));

            return role;
        }
    }
}
