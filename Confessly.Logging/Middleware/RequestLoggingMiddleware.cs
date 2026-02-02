using Confessly.Logging.Extensions;
using Confessly.Logging.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Confessly.Logging.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses with correlation tracking.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggingService _loggingService;
    private readonly ICorrelationService _correlationService;
    private readonly IPerformanceLogger _performanceLogger;

    public RequestLoggingMiddleware(RequestDelegate next, 
        ILoggingService loggingService, 
        ICorrelationService correlationService,
        IPerformanceLogger performanceLogger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        _correlationService = correlationService ?? throw new ArgumentNullException(nameof(correlationService));
        _performanceLogger = performanceLogger ?? throw new ArgumentNullException(nameof(performanceLogger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Set or generate correlation ID
        var correlationId = GetOrGenerateCorrelationId(context);
        _correlationService.SetCorrelationId(correlationId);

        // Add correlation ID to response headers
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var request = context.Request;
        var requestPath = $"{request.Method} {request.Path}";

        // Start performance tracking
        using var performanceTracker = _performanceLogger.TrackApiEndpoint(
            request.Method, 
            request.Path, 
            GetUserId(context));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Log request start
            using (_loggingService.BeginScope("RequestProcessing", correlationId))
            {
                _loggingService.LogInformation("Started processing request {RequestPath} from {RemoteIpAddress}", 
                    requestPath, 
                    GetRemoteIpAddress(context));

                await _next(context);

                stopwatch.Stop();

                // Log successful response
                _loggingService.LogInformation("Completed request {RequestPath} with status {StatusCode} in {Duration}ms", 
                    requestPath, 
                    context.Response.StatusCode, 
                    stopwatch.ElapsedMilliseconds);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log request failure
            var properties = new Dictionary<string, object>
            {
                ["RequestPath"] = requestPath,
                ["StatusCode"] = context.Response.StatusCode,
                ["Duration"] = stopwatch.ElapsedMilliseconds,
                ["RemoteIpAddress"] = GetRemoteIpAddress(context),
                ["UserAgent"] = context.Request.Headers["User-Agent"].ToString()
            };

            using (_loggingService.BeginScope(properties))
            {
                _loggingService.LogError(ex, "Request {RequestPath} failed after {Duration}ms", 
                    requestPath, 
                    stopwatch.ElapsedMilliseconds);
            }

            throw; // Re-throw the exception
        }
    }

    private string GetOrGenerateCorrelationId(HttpContext context)
    {
        // Check for existing correlation ID in headers
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId) && 
            !string.IsNullOrWhiteSpace(correlationId))
        {
            return correlationId.ToString();
        }

        // Generate new correlation ID
        return _correlationService.GenerateCorrelationId();
    }

    private static string GetRemoteIpAddress(HttpContext context)
    {
        // Check for forwarded IP address (when behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private static object? GetUserId(HttpContext context)
    {
        // Try to get user ID from claims
        var userIdClaim = context.User?.FindFirst("sub") ?? 
                         context.User?.FindFirst("userId") ?? 
                         context.User?.FindFirst("id");

        return userIdClaim?.Value;
    }
}