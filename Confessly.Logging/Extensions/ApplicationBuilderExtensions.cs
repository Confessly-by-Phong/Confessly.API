using Confessly.Logging.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Confessly.Logging.Extensions;

/// <summary>
/// Extension methods for adding Confessly logging middleware.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds request logging middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}