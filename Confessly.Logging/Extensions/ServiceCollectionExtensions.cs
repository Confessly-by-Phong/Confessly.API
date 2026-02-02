using Confessly.Logging.Interfaces;
using Confessly.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Confessly.Logging.Extensions;

/// <summary>
/// Extension methods for registering Confessly logging services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Confessly logging services to the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddConfesslyLogging(this IServiceCollection services)
    {
        // Register core logging services
        services.AddSingleton<ICorrelationService, CorrelationService>();
        services.AddSingleton<ILoggingService, SerilogService>();
        services.AddSingleton<IPerformanceLogger, PerformanceLogger>();

        return services;
    }

    /// <summary>
    /// Configures Serilog for the application with sensible defaults.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="configureLogger">Optional additional logger configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddConfesslySerilog(this IServiceCollection services, 
        IHostEnvironment hostEnvironment, 
        Action<LoggerConfiguration>? configureLogger = null)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId();

        // Configure sinks based on environment
        if (hostEnvironment.IsDevelopment())
        {
            loggerConfig
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}"
                );
        }
        else
        {
            loggerConfig
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}"
                );
        }

        // Allow custom configuration
        configureLogger?.Invoke(loggerConfig);

        // Configure Serilog as the logging provider
        Log.Logger = loggerConfig.CreateLogger();

        return services;
    }

    /// <summary>
    /// Configures Serilog with Azure Application Insights support.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="applicationInsightsConnectionString">Application Insights connection string.</param>
    /// <param name="configureLogger">Optional additional logger configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddConfesslySerilogWithApplicationInsights(this IServiceCollection services, 
        IHostEnvironment hostEnvironment, 
        string applicationInsightsConnectionString,
        Action<LoggerConfiguration>? configureLogger = null)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId();

        // Console sink for all environments
        if (hostEnvironment.IsDevelopment())
        {
            loggerConfig
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}"
                );
        }
        else
        {
            loggerConfig
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj} {Properties:j}{NewLine}{Exception}"
                );
        }

        // Application Insights sink for non-development environments
        if (!hostEnvironment.IsDevelopment() && !string.IsNullOrWhiteSpace(applicationInsightsConnectionString))
        {
            loggerConfig.WriteTo.ApplicationInsights(applicationInsightsConnectionString, TelemetryConverter.Traces);
        }

        // Allow custom configuration
        configureLogger?.Invoke(loggerConfig);

        // Configure Serilog as the logging provider
        Log.Logger = loggerConfig.CreateLogger();

        return services;
    }
}