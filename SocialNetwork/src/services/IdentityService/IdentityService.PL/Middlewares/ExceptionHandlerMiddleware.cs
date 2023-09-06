using IdentityService.BLL.Exceptions;

namespace IdentityService.PL.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.Headers.ContentType = "text/json; charset=utf-8";
                context.Response.StatusCode = 404;
                var response = new { ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (AlreadyExistsException ex)
            {
                context.Response.Headers.ContentType = "text/json; charset=utf-8";
                context.Response.StatusCode = 409;
                var response = new { ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.Headers.ContentType = "text/json; charset=utf-8";
                context.Response.StatusCode = 401;
                var response = new { ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
