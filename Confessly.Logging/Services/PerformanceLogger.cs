using Confessly.Logging.Interfaces;
using System.Diagnostics;

namespace Confessly.Logging.Services;

/// <summary>
/// Disposable operation tracker that measures timing.
/// </summary>
class OperationTracker : IDisposable
{
    private readonly ILoggingService _loggingService;
    private readonly string _operationName;
    private readonly Dictionary<string, object>? _additionalProperties;
    private readonly Stopwatch _stopwatch;
    private bool _disposed = false;

    public OperationTracker(ILoggingService loggingService, string operationName, Dictionary<string, object>? additionalProperties)
    {
        _loggingService = loggingService;
        _operationName = operationName;
        _additionalProperties = additionalProperties;
        _stopwatch = Stopwatch.StartNew();

        _loggingService.LogDebug("Started operation {OperationName}", operationName);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _stopwatch.Stop();
            var duration = _stopwatch.Elapsed;

            var properties = new Dictionary<string, object>
            {
                ["OperationName"] = _operationName,
                ["Duration"] = duration.TotalMilliseconds,
                ["DurationMs"] = Math.Round(duration.TotalMilliseconds, 2)
            };

            if (_additionalProperties != null)
            {
                foreach (var prop in _additionalProperties)
                {
                    properties[prop.Key] = prop.Value;
                }
            }

            using (_loggingService.BeginScope(properties))
            {
                _loggingService.LogInformation("Completed operation {OperationName} in {Duration}ms",
                    _operationName, duration.TotalMilliseconds);
            }

            _disposed = true;
        }
    }
}

/// <summary>
/// Performance logger implementation that tracks operation timing and outcomes.
/// </summary>
public class PerformanceLogger : IPerformanceLogger
{
    private readonly ILoggingService _loggingService;

    public PerformanceLogger(ILoggingService loggingService)
    {
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    }

    /// <inheritdoc />
    public IDisposable TrackOperation(string operationName, Dictionary<string, object>? additionalProperties = null)
    {
        return new OperationTracker(_loggingService, operationName, additionalProperties);
    }

    /// <inheritdoc />
    public void LogOperationDuration(string operationName, TimeSpan duration, Dictionary<string, object>? additionalProperties = null)
    {
        var properties = CreateLogProperties(operationName, duration, additionalProperties);
        _loggingService.LogInformation("Operation {OperationName} completed in {Duration}ms", 
            operationName, duration.TotalMilliseconds);
    }

    /// <inheritdoc />
    public void LogOperationSuccess(string operationName, TimeSpan duration, Dictionary<string, object>? additionalProperties = null)
    {
        var properties = CreateLogProperties(operationName, duration, additionalProperties);
        properties["Status"] = "Success";
        
        using (_loggingService.BeginScope(properties))
        {
            _loggingService.LogInformation("Operation {OperationName} succeeded in {Duration}ms", 
                operationName, duration.TotalMilliseconds);
        }
    }

    /// <inheritdoc />
    public void LogOperationFailure(string operationName, TimeSpan duration, Exception exception, Dictionary<string, object>? additionalProperties = null)
    {
        var properties = CreateLogProperties(operationName, duration, additionalProperties);
        properties["Status"] = "Failed";
        properties["ExceptionType"] = exception.GetType().Name;
        
        using (_loggingService.BeginScope(properties))
        {
            _loggingService.LogError(exception, "Operation {OperationName} failed after {Duration}ms", 
                operationName, duration.TotalMilliseconds);
        }
    }

    private static Dictionary<string, object> CreateLogProperties(string operationName, TimeSpan duration, Dictionary<string, object>? additionalProperties)
    {
        var properties = new Dictionary<string, object>
        {
            ["OperationName"] = operationName,
            ["Duration"] = duration.TotalMilliseconds,
            ["DurationMs"] = Math.Round(duration.TotalMilliseconds, 2)
        };

        if (additionalProperties != null)
        {
            foreach (var prop in additionalProperties)
            {
                properties[prop.Key] = prop.Value;
            }
        }

        return properties;
    }
}