using Confessly.Logging.Interfaces;

namespace Confessly.Logging.Services;

/// <summary>
/// Thread-safe correlation service that manages correlation IDs across request boundaries.
/// </summary>
public class CorrelationService : ICorrelationService
{
    private static readonly AsyncLocal<string?> _correlationId = new();

    /// <inheritdoc />
    public string CorrelationId => _correlationId.Value ?? GenerateCorrelationId();

    /// <inheritdoc />
    public void SetCorrelationId(string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
            throw new ArgumentException("Correlation ID cannot be null or whitespace.", nameof(correlationId));

        _correlationId.Value = correlationId;
    }

    /// <inheritdoc />
    public string GenerateCorrelationId()
    {
        var correlationId = Guid.NewGuid().ToString("N")[..8]; // Short correlation ID
        _correlationId.Value = correlationId;
        return correlationId;
    }
}