using ChatService.Application.Exceptions;
using FluentValidation;
using System;

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
                context.Response.StatusCode = ex switch
                {
                    NotFoundException => 404,
                    AlreadyExistsException => 409,
                    ForbiddenException => 403,
                    ValidationException => 400,
                    _ => 500
                };
                context.Response.Headers.ContentType = "text/json; charset=utf-8";

                if (ex is ValidationException validationException)
                {
                    var errors = validationException.Errors.ToDictionary(e => e.PropertyName.Remove(0, 4), e => e.ErrorMessage.Remove(1, 5));
                    var validationResponse = new
                    {
                        Message = errors
                    };
                    await context.Response.WriteAsJsonAsync(validationResponse);
                }
                else
                {
                    var response = new { ex.Message };
                    await context.Response.WriteAsJsonAsync(response);
                }
            }
        }
    }
}
