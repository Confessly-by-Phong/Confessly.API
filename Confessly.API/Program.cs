using Confessly.Configuration;
using Confessly.Logging.Extensions;
using Serilog;

namespace Confessly.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();

            // Configure Serilog
            builder.Services.AddConfesslySerilog(builder.Environment);
            builder.Services.AddConfesslyLogging();

            ConfesslyConfiguration.Initialize(builder.Configuration);

            // Clear default logging providers and use Serilog
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog();

            builder.Services.AddConfesslyDependencyInjection(builder.Environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            // Add exception handling middleware early in the pipeline to catch unhandled exceptions
            app.UseExceptionHandler(_ => { });

            // Add request logging middleware early in the pipeline
            app.UseRequestLogging();


            app.UseHttpsRedirection();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            try
            {
                Log.Information("Starting Confessly API application");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
