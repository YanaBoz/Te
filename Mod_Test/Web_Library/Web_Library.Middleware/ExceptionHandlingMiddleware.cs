using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Web_Library.Middleware.Exceptions;

namespace Web_Library.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex) when (IsClientCanceled(context, ex))
            {
                _logger.LogWarning("Request was aborted by the client."); 
                context.Response.StatusCode = 499;
                return;
            }
            catch (Exception exception)
            {
                LogException(exception);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = GetStatusCode(exception);

                var result = JsonSerializer.Serialize(new
                {
                    error = new
                    {
                        message = exception.Message,
                        type = exception.GetType().Name
                    }
                });

                await context.Response.WriteAsync(result);
            }
        }

        private void LogException(Exception exception)
        {
            switch (exception)
            {
                case NotFoundException:
                case AlreadyExistsException:
                case BadRequestException:
                case UnauthorizedException:
                case ForbiddenException:
                    _logger.LogWarning(exception, "Handled application error: {Message}", exception.Message);
                    break;
                default:
                    _logger.LogError(exception, "An unhandled exception occurred.");
                    break;
            }
        }

        private static bool IsClientCanceled(HttpContext context, Exception ex)
        {
            return ex is OperationCanceledException && context.RequestAborted.IsCancellationRequested;
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                AlreadyExistsException => StatusCodes.Status409Conflict,
                BadRequestException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                OperationCanceledException => StatusCodes.Status408RequestTimeout,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
