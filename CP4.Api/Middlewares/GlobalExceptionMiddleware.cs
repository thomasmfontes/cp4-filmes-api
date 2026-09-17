using CP4.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace CP4.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainValidationException ex)
            {
                _logger.LogWarning("Violacao de regra de negocio: {Mensagem}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno nao tratado: {Mensagem}", ex.Message);

                var errorDetails = _env.IsDevelopment()
                    ? $"{ex.Message}{(ex.InnerException != null ? " --> " + ex.InnerException.Message : "")}"
                    : "Ocorreu um erro interno no servidor. Tente novamente mais tarde.";

                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, errorDetails);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = (int)statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
