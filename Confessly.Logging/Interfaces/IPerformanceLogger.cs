namespace Confessly.Logging.Interfaces;

/// <summary>
/// Service for performance logging and monitoring.
/// </summary>
public interface IPerformanceLogger
{
    /// <summary>
    /// Starts tracking performance for an operation.
    /// </summary>
    /// <param name="operationName">Name of the operation being tracked.</param>
    /// <param name="additionalProperties">Additional properties to log with the performance data.</param>
    /// <returns>A disposable object that stops tracking when disposed.</returns>
    IDisposable TrackOperation(string operationName, Dictionary<string, object>? additionalProperties = null);

    /// <summary>
    /// Logs the duration of an operation.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <param name="duration">Duration of the operation.</param>
    /// <param name="additionalProperties">Additional properties to log.</param>
    void LogOperationDuration(string operationName, TimeSpan duration, Dictionary<string, object>? additionalProperties = null);

    /// <summary>
    /// Logs when an operation completes successfully.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <param name="duration">Duration of the operation.</param>
    /// <param name="additionalProperties">Additional properties to log.</param>
    void LogOperationSuccess(string operationName, TimeSpan duration, Dictionary<string, object>? additionalProperties = null);

    /// <summary>
    /// Logs when an operation fails.
    /// </summary>
    /// <param name="operationName">Name of the operation.</param>
    /// <param name="duration">Duration before failure.</param>
    /// <param name="exception">Exception that caused the failure.</param>
    /// <param name="additionalProperties">Additional properties to log.</param>
    void LogOperationFailure(string operationName, TimeSpan duration, Exception exception, Dictionary<string, object>? additionalProperties = null);
}