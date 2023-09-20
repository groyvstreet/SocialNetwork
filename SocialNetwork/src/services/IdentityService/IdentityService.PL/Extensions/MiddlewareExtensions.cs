using IdentityService.PL.Middlewares;

namespace IdentityService.PL.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
