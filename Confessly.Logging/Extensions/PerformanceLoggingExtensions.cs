using Confessly.Logging.Interfaces;

namespace Confessly.Logging.Extensions;

/// <summary>
/// Extension methods for performance logging operations.
/// </summary>
public static class PerformanceLoggingExtensions
{
    /// <summary>
    /// Tracks the performance of a database operation.
    /// </summary>
    /// <param name="performanceLogger">The performance logger.</param>
    /// <param name="operation">The database operation name.</param>
    /// <param name="entityType">The type of entity being operated on.</param>
    /// <param name="recordCount">Number of records affected (optional).</param>
    /// <returns>Disposable tracker for the operation.</returns>
    public static IDisposable TrackDatabaseOperation(this IPerformanceLogger performanceLogger, 
        string operation, 
        string entityType, 
        int? recordCount = null)
    {
        var properties = new Dictionary<string, object>
        {
            ["OperationType"] = "Database",
            ["EntityType"] = entityType,
            ["DatabaseOperation"] = operation
        };

        if (recordCount.HasValue)
        {
            properties["RecordCount"] = recordCount.Value;
        }

        return performanceLogger.TrackOperation($"DB_{operation}_{entityType}", properties);
    }

    /// <summary>
    /// Tracks the performance of an API endpoint.
    /// </summary>
    /// <param name="performanceLogger">The performance logger.</param>
    /// <param name="httpMethod">The HTTP method.</param>
    /// <param name="endpoint">The API endpoint.</param>
    /// <param name="userId">The user ID if available.</param>
    /// <returns>Disposable tracker for the operation.</returns>
    public static IDisposable TrackApiEndpoint(this IPerformanceLogger performanceLogger, 
        string httpMethod, 
        string endpoint, 
        object? userId = null)
    {
        var properties = new Dictionary<string, object>
        {
            ["OperationType"] = "API",
            ["HttpMethod"] = httpMethod,
            ["Endpoint"] = endpoint
        };

        if (userId != null)
        {
            properties["UserId"] = userId;
        }

        return performanceLogger.TrackOperation($"API_{httpMethod}_{endpoint}", properties);
    }

    /// <summary>
    /// Tracks the performance of a business logic operation.
    /// </summary>
    /// <param name="performanceLogger">The performance logger.</param>
    /// <param name="operationName">Name of the business operation.</param>
    /// <param name="additionalContext">Additional context for the operation.</param>
    /// <returns>Disposable tracker for the operation.</returns>
    public static IDisposable TrackBusinessOperation(this IPerformanceLogger performanceLogger, 
        string operationName, 
        Dictionary<string, object>? additionalContext = null)
    {
        var properties = new Dictionary<string, object>
        {
            ["OperationType"] = "Business"
        };

        if (additionalContext != null)
        {
            foreach (var prop in additionalContext)
            {
                properties[prop.Key] = prop.Value;
            }
        }

        return performanceLogger.TrackOperation($"BIZ_{operationName}", properties);
    }

    /// <summary>
    /// Logs repository operation metrics.
    /// </summary>
    /// <param name="loggingService">The logging service.</param>
    /// <param name="operation">The repository operation.</param>
    /// <param name="entityType">The entity type.</param>
    /// <param name="recordCount">Number of records affected.</param>
    /// <param name="duration">Duration of the operation.</param>
    public static void LogRepositoryMetrics(this ILoggingService loggingService, 
        string operation, 
        string entityType, 
        int recordCount, 
        TimeSpan duration)
    {
        var properties = new Dictionary<string, object>
        {
            ["Component"] = "Repository",
            ["Operation"] = operation,
            ["EntityType"] = entityType,
            ["RecordCount"] = recordCount,
            ["Duration"] = duration.TotalMilliseconds,
            ["RecordsPerSecond"] = recordCount > 0 ? Math.Round(recordCount / duration.TotalSeconds, 2) : 0
        };

        using (loggingService.BeginScope(properties))
        {
            loggingService.LogInformation("Repository {Operation} on {EntityType} processed {RecordCount} records in {Duration}ms", 
                operation, entityType, recordCount, duration.TotalMilliseconds);
        }
    }
}