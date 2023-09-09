using IdentityService.BLL.Exceptions;
using Microsoft.IdentityModel.Tokens;

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
            catch (Exception ex)
            {
                switch (ex)
                {
                    case NotFoundException:
                        context.Response.StatusCode = 404;
                        break;
                    case AlreadyExistsException:
                        context.Response.StatusCode = 409;
                        break;
                    case ForbiddenException:
                        context.Response.StatusCode = 403;
                        break;
                    case SecurityTokenExpiredException:
                        context.Response.StatusCode = 401;
                        break;
                    case SecurityTokenException:
                        context.Response.StatusCode = 401;
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        break;
                }

                context.Response.Headers.ContentType = "text/json; charset=utf-8";
                var response = new { ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
