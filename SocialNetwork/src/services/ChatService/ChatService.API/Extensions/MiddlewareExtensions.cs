using ChatService.API.Middlewares;

namespace ChatService.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
