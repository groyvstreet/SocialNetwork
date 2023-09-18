﻿using ChatService.Application.Exceptions;

namespace ChatService.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
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
