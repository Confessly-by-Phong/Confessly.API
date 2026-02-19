using Confessly.Contracts.Core;
using Confessly.Messages;
using Confessly.Validators;
using Microsoft.AspNetCore.Diagnostics;

namespace Confessly.API.Middlewares
{
    public class ExceptionHandlingMiddleware : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
            CancellationToken cancellationToken)
        {
            string traceId = httpContext.TraceIdentifier;

            ConfesslyResponse<object> response;

            switch (exception)
            {
                case ConfesslyValidationException validationException:
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = ConfesslyResponse<object>.Fail(validationException.Message, traceId);
                    break;
                default:
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = ConfesslyResponse<object>.Fail(
                        string.Format(ConfesslyExceptionMessages.InternalServerError, exception.Message), traceId);
                    break;
            }
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}
