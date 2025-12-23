using FraudSys.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace FraudSys.API.Middlewares
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new
            {
                message = exception.Message,
                statusCode = (int)HttpStatusCode.InternalServerError
            };

            switch (exception)
            {
                case AccountLimitNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new
                    {
                        message = exception.Message,
                        statusCode = (int)HttpStatusCode.NotFound
                    };
                    break;

                case InvalidDocumentException:
                case InsufficientLimitException:
                case DomainException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new
                    {
                        message = exception.Message,
                        statusCode = (int)HttpStatusCode.BadRequest
                    };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new
                    {
                        message = "Ocorreu um erro interno no servidor",
                        statusCode = (int)HttpStatusCode.InternalServerError,
                    };
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }
}
