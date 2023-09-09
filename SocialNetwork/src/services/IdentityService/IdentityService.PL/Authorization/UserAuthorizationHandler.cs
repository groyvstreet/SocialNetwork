using IdentityService.DAL.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;

namespace IdentityService.PL.Authorization
{
    public class UserAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       string resource)
        {
            if (requirement.Name == Operations.Create.Name)
            {
                if (resource != Roles.Admin || context.User.IsInRole(Roles.Admin))
                {
                    context.Succeed(requirement);
                }
            }

            if (requirement.Name == Operations.Update.Name ||
                requirement.Name == Operations.Delete.Name)
            {
                if (context.User.FindFirstValue(ClaimTypes.NameIdentifier) == resource ||
                    context.User.IsInRole(Roles.Admin))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
