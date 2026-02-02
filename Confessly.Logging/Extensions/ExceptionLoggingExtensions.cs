using Confessly.Logging.Interfaces;

namespace Confessly.Logging.Extensions;

/// <summary>
/// Extension methods for enhanced exception logging.
/// </summary>
public static class ExceptionLoggingExtensions
{
    /// <summary>
    /// Logs an exception with detailed context information.
    /// </summary>
    /// <param name="loggingService">The logging service.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="contextMessage">Additional context about when/where the exception occurred.</param>
    /// <param name="additionalProperties">Additional properties to include in the log.</param>
    public static void LogExceptionWithContext(this ILoggingService loggingService, 
        Exception exception, 
        string contextMessage, 
        Dictionary<string, object>? additionalProperties = null)
    {
        var properties = new Dictionary<string, object>
        {
            ["ExceptionType"] = exception.GetType().Name,
            ["ExceptionMessage"] = exception.Message,
            ["StackTrace"] = exception.StackTrace ?? "No stack trace available"
        };

        if (exception.InnerException != null)
        {
            properties["InnerExceptionType"] = exception.InnerException.GetType().Name;
            properties["InnerExceptionMessage"] = exception.InnerException.Message;
        }

        // Add any additional properties
        if (additionalProperties != null)
        {
            foreach (var prop in additionalProperties)
            {
                properties[prop.Key] = prop.Value;
            }
        }

        using (loggingService.BeginScope(properties))
        {
            loggingService.LogError(exception, contextMessage);
        }
    }

    /// <summary>
    /// Logs an exception that occurred during a repository operation.
    /// </summary>
    /// <param name="loggingService">The logging service.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="operation">The repository operation that failed.</param>
    /// <param name="entityType">The type of entity involved.</param>
    /// <param name="entityId">The ID of the entity if available.</param>
    public static void LogRepositoryException(this ILoggingService loggingService, 
        Exception exception, 
        string operation, 
        string entityType, 
        object? entityId = null)
    {
        var additionalProperties = new Dictionary<string, object>
        {
            ["Operation"] = operation,
            ["EntityType"] = entityType,
            ["Component"] = "Repository"
        };

        if (entityId != null)
        {
            additionalProperties["EntityId"] = entityId;
        }

        loggingService.LogExceptionWithContext(exception, 
            "Repository operation {Operation} failed for {EntityType}", 
            additionalProperties);
    }

    /// <summary>
    /// Logs an exception that occurred during an API operation.
    /// </summary>
    /// <param name="loggingService">The logging service.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="httpMethod">The HTTP method.</param>
    /// <param name="endpoint">The API endpoint.</param>
    /// <param name="userId">The user ID if available.</param>
    public static void LogApiException(this ILoggingService loggingService, 
        Exception exception, 
        string httpMethod, 
        string endpoint, 
        object? userId = null)
    {
        var additionalProperties = new Dictionary<string, object>
        {
            ["HttpMethod"] = httpMethod,
            ["Endpoint"] = endpoint,
            ["Component"] = "API"
        };

        if (userId != null)
        {
            additionalProperties["UserId"] = userId;
        }

        loggingService.LogExceptionWithContext(exception, 
            "API operation {HttpMethod} {Endpoint} failed", 
            additionalProperties);
    }

    /// <summary>
    /// Logs validation errors in a structured way.
    /// </summary>
    /// <param name="loggingService">The logging service.</param>
    /// <param name="validationErrors">Dictionary of validation errors.</param>
    /// <param name="operation">The operation that had validation errors.</param>
    /// <param name="additionalContext">Additional context information.</param>
    public static void LogValidationErrors(this ILoggingService loggingService, 
        Dictionary<string, string[]> validationErrors, 
        string operation, 
        Dictionary<string, object>? additionalContext = null)
    {
        var properties = new Dictionary<string, object>
        {
            ["Operation"] = operation,
            ["ValidationErrors"] = validationErrors,
            ["ErrorCount"] = validationErrors.Values.SelectMany(x => x).Count(),
            ["Component"] = "Validation"
        };

        if (additionalContext != null)
        {
            foreach (var prop in additionalContext)
            {
                properties[prop.Key] = prop.Value;
            }
        }

        using (loggingService.BeginScope(properties))
        {
            loggingService.LogWarning("Validation failed for operation {Operation} with {ErrorCount} errors", 
                operation, properties["ErrorCount"]);
        }
    }
}