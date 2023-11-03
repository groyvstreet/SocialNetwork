using Hangfire.Dashboard;
using System.Security.Claims;

namespace ChatService.API.Hangfire
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            
            return (httpContext.User.Identity?.IsAuthenticated ?? false) &&
                httpContext.User.FindFirstValue(ClaimTypes.Role) == "admin";
        }
    }
}
