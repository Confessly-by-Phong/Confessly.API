namespace Confessly.Logging.Interfaces;

/// <summary>
/// Service for managing correlation IDs across request boundaries.
/// </summary>
public interface ICorrelationService
{
    /// <summary>
    /// Gets the current correlation ID for the request.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Sets a new correlation ID for the current context.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set.</param>
    void SetCorrelationId(string correlationId);

    /// <summary>
    /// Generates a new correlation ID and sets it as current.
    /// </summary>
    /// <returns>The newly generated correlation ID.</returns>
    string GenerateCorrelationId();
}