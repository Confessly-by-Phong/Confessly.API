using Microsoft.Extensions.Logging;

namespace Confessly.Logging.Interfaces;

/// <summary>
/// Enhanced logging service interface that provides structured logging capabilities
/// with correlation tracking and performance monitoring.
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Arguments for the message template.</param>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Arguments for the message template.</param>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Arguments for the message template.</param>
    void LogError(string message, params object[] args);

    /// <summary>
    /// Logs an error with exception details.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Additional context message.</param>
    /// <param name="args">Arguments for the message template.</param>
    void LogError(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs a debug message (only in development).
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Arguments for the message template.</param>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Creates a scoped logger with additional properties.
    /// </summary>
    /// <param name="properties">Properties to add to all log entries in this scope.</param>
    /// <returns>A disposable scope that enriches all log entries.</returns>
    IDisposable BeginScope(Dictionary<string, object> properties);

    /// <summary>
    /// Creates a scoped logger for a specific operation.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <param name="operationId">Unique identifier for the operation.</param>
    /// <returns>A disposable scope that tracks the operation.</returns>
    IDisposable BeginScope(string operationName, string? operationId = null);
}