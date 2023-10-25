using PostService.Application.Exceptions;

namespace PostService.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = ex switch
                {
                    NotFoundException => 404,
                    AlreadyExistsException => 409,
                    ForbiddenException => 403,
                    _ => 500,
                };
                context.Response.Headers.ContentType = "text/json; charset=utf-8";
                var response = new { ex.Message };
                await context.Response.WriteAsJsonAsync(response);

                logger.LogError("exception {exception}", ex.ToString());
            }
        }
    }
}
