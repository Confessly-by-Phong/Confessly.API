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

            // Configure Serilog
            builder.Services.AddConfesslySerilog(builder.Environment);
            builder.Services.AddConfesslyLogging();

            // Clear default logging providers and use Serilog
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
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
