using System.Net;
using System.Text.Json;
using ApiGenerico.WebAPI.Models;

namespace ApiGenerico.WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Se produjo un error de negocio en la solicitud {TraceId}.", context.TraceIdentifier);
                await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Se produjo un error no controlado en la solicitud {TraceId}.", context.TraceIdentifier);
                await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "Ocurrió un error interno al procesar la solicitud.");
            }
        }

        private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var payload = JsonSerializer.Serialize(new ApiErrorResponse
            {
                Message = message,
                TraceId = context.TraceIdentifier
            });

            await context.Response.WriteAsync(payload);
        }
    }
}
