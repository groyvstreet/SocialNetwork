﻿using IdentityService.BLL.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.PL.Middlewares
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
                    SecurityTokenExpiredException => 401,
                    SecurityTokenException => 401,
                    _ => 500,
                };
                context.Response.Headers.ContentType = "text/json; charset=utf-8";
                var response = new { ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
