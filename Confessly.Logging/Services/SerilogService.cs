using Confessly.Logging.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Confessly.Logging.Services;

/// <summary>
/// Serilog-based implementation of the logging service.
/// </summary>
public class SerilogService : ILoggingService
{
    private readonly ILogger<SerilogService> _logger;
    private readonly ICorrelationService _correlationService;

    public SerilogService(ILogger<SerilogService> logger, ICorrelationService correlationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _correlationService = correlationService ?? throw new ArgumentNullException(nameof(correlationService));
    }

    /// <inheritdoc />
    public void LogInformation(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", _correlationService.CorrelationId))
        {
            _logger.LogInformation(message, args);
        }
    }

    /// <inheritdoc />
    public void LogWarning(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", _correlationService.CorrelationId))
        {
            _logger.LogWarning(message, args);
        }
    }

    /// <inheritdoc />
    public void LogError(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", _correlationService.CorrelationId))
        {
            _logger.LogError(message, args);
        }
    }

    /// <inheritdoc />
    public void LogError(Exception exception, string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", _correlationService.CorrelationId))
        {
            _logger.LogError(exception, message, args);
        }
    }

    /// <inheritdoc />
    public void LogDebug(string message, params object[] args)
    {
        using (LogContext.PushProperty("CorrelationId", _correlationService.CorrelationId))
        {
            _logger.LogDebug(message, args);
        }
    }

    /// <inheritdoc />
    public IDisposable BeginScope(Dictionary<string, object> properties)
    {
        var enrichers = new List<IDisposable>();
        
        foreach (var property in properties)
        {
            enrichers.Add(LogContext.PushProperty(property.Key, property.Value));
        }

        return new CompositeDisposable(enrichers);
    }

    /// <inheritdoc />
    public IDisposable BeginScope(string operationName, string? operationId = null)
    {
        var properties = new Dictionary<string, object>
        {
            ["OperationName"] = operationName,
            ["OperationId"] = operationId ?? Guid.NewGuid().ToString(),
            ["CorrelationId"] = _correlationService.CorrelationId
        };

        return BeginScope(properties);
    }

    /// <summary>
    /// Composite disposable to handle multiple disposable objects.
    /// </summary>
    private class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;
        private bool _disposed = false;

        public CompositeDisposable(List<IDisposable> disposables)
        {
            _disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var disposable in _disposables)
                {
                    disposable?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}