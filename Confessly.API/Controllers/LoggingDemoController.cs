using Confessly.Logging.Extensions;
using Confessly.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Confessly.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoggingDemoController : ControllerBase
{
    private readonly ILoggingService _logger;
    private readonly IPerformanceLogger _performanceLogger;

    public LoggingDemoController(ILoggingService logger, IPerformanceLogger performanceLogger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _performanceLogger = performanceLogger ?? throw new ArgumentNullException(nameof(performanceLogger));
    }

    [HttpGet("info")]
    public IActionResult LogInformation()
    {
        _logger.LogInformation("This is an information log from the demo controller");
        return Ok(new { message = "Information logged successfully" });
    }

    [HttpGet("warning")]
    public IActionResult LogWarning()
    {
        _logger.LogWarning("This is a warning log - something might need attention");
        return Ok(new { message = "Warning logged successfully" });
    }

    [HttpGet("error")]
    public IActionResult LogError()
    {
        try
        {
            // Simulate an error
            throw new InvalidOperationException("This is a simulated error for logging demonstration");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in the demo controller while processing request");
            return BadRequest(new { message = "Error logged successfully", error = ex.Message });
        }
    }

    [HttpGet("performance")]
    public async Task<IActionResult> LogPerformance()
    {
        using var performanceTracker = _performanceLogger.TrackApiEndpoint("GET", "/api/loggingdemo/performance");

        // Simulate some work
        await Task.Delay(100);

        _logger.LogInformation("Performance tracking completed for demo endpoint");
        return Ok(new { message = "Performance logged successfully" });
    }

    [HttpGet("scoped")]
    public IActionResult LogWithScope()
    {
        var properties = new Dictionary<string, object>
        {
            ["UserId"] = "demo-user-123",
            ["Operation"] = "ScopedLogging",
            ["RequestId"] = HttpContext.TraceIdentifier
        };

        using (_logger.BeginScope(properties))
        {
            _logger.LogInformation("This log entry will include the scoped properties");
            _logger.LogInformation("All logs within this scope will have the additional context");
        }

        return Ok(new { message = "Scoped logging demonstrated successfully" });
    }

    [HttpGet("validation-error")]
    public IActionResult LogValidationError()
    {
        var validationErrors = new Dictionary<string, string[]>
        {
            ["Email"] = ["Email is required", "Email format is invalid"],
            ["Password"] = ["Password must be at least 8 characters"],
            ["Name"] = ["Name is required"]
        };

        _logger.LogValidationErrors(validationErrors, "UserRegistration", new Dictionary<string, object>
        {
            ["Endpoint"] = "/api/users/register",
            ["AttemptedEmail"] = "invalid-email@"
        });

        return BadRequest(new { message = "Validation errors logged", errors = validationErrors });
    }
}