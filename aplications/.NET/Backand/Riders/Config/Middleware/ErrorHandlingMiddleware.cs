using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Dto.RequestResultsDto;

namespace Riders.Config.Middleware
{
    public sealed class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteRequestResultsDtoAsync(context, StatusCodes.Status204NoContent, ex.Message, false, null);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ArgumentException capturada");
                await WriteRequestResultsDtoAsync(context, StatusCodes.Status400BadRequest, ex.Message, false, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção não tratada");
                await WriteRequestResultsDtoAsync(context, StatusCodes.Status500InternalServerError, "Internal server error.", false, null);
            }
            
        }

        private async Task WriteRequestResultsDtoAsync(HttpContext context, int statusCode, string message, bool success, object? data)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json; charset=utf-8";
            }

            var payload = new RequestResultsDto
            {
                Message = message,
                Success = success,
                Data = data
            };

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}