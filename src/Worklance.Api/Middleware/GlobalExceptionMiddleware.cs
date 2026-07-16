using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.Exceptions;
using Microsoft.AspNetCore.Http;


namespace Worklance.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Microsoft.Extensions.Logging.ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, Microsoft.Extensions.Logging.ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "A fatal unhandled exception occurred during the request.");
                await HandleExceptionAsync(context, exception);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (exception)
            {
                case BadRequestException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = exception.Message;
                    break;

                case ForbiddenException:
                    statusCode = StatusCodes.Status403Forbidden;
                    message = exception.Message;
                    break;

                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = ApiResponse<object>.Failure(
                message,
                statusCode);

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
