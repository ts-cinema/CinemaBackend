using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Serilog;
using Template.Service.Configuration;
using Template.Service.Setup;

namespace Template.Service
{
    /// <summary>
    /// Represents the main entry point for the Template service.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static DateTime? _startTime = null;
        private static string _appInsightsInstrumentationKey = null;

        /// <summary>
        /// The main entry point for the service.
        /// </summary>
        /// <param name="args">
        /// An array of strings containing the command line arguements.
        /// </param>
        internal static void Main(string[] args)
        {
            Program._startTime = DateTime.UtcNow;

            // Needed only to get Azure Application Insights Instrumentation Key.
            // It is needed prior to the configuration variable defined below.
            var tempEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var tempConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{tempEnv}.json", optional: true)
                .Build();
            Program._appInsightsInstrumentationKey = tempConfig.GetValue<string>("ApplicationInsights:InstrumentationKey");

            // Create and configure the web service
            var webHost = CreateWebHostBuilder(args).Build();

            // Get access to required services
            var logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.ApplicationInsights(new TelemetryConfiguration { InstrumentationKey = _appInsightsInstrumentationKey }, TelemetryConverter.Traces)
               .CreateLogger();
            var configuration = webHost.Services.GetRequiredService<IConfiguration>();

            logger?.Information($"The {configuration.GetServiceName()} service is starting (Process Name: {Process.GetCurrentProcess().ProcessName}, Process ID: {Process.GetCurrentProcess().Id}).");

            // Perform any service initialization
            OnStartAsync(logger, configuration).GetAwaiter().GetResult();

            logger?.Information($"The {configuration.GetServiceName()} service has started");

            // Run the web service
            webHost.Run();
        }

        /// <summary>
        /// Configures and launches the web host.
        /// </summary>
        /// <param name="args">
        /// An array of strings containing the command line arguements.
        /// </param>
        /// <returns>
        /// A builder for the web host.
        /// </returns>
        internal static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Add support for JSON settings from a JSON file
                    var env = context.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                    // Add support for environment variables that override JSON settings
                    config.AddEnvironmentVariables(prefix: "Envista_");

                    // Add support for command line variables that override JSON and environment variable settings
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseStartup<Startup>();

        /// <summary>
        /// Gets or sets the service's configuration properties.
        /// </summary>
        /// <remarks>
        /// Please do not use this property directly; instead use dependency injection to get access to the service's configuration.
        /// </remarks>
        internal static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the service's logger.
        /// </summary>
        /// <remarks>
        /// Please do not use this property directly; instead use dependency injection to gain access to the service's logging capabilities.
        /// </remarks>
        internal static Microsoft.Extensions.Logging.ILogger Logger { get; set; }

        /// <summary>
        /// Performs any special processing when the service is started.
        /// </summary>
        /// <param name="logger">
        /// An object that represents the service's logger.
        /// </param>
        /// <param name="configuration">
        /// An object that represents the service's configuration.
        /// </param>
        internal static async Task OnStartAsync(Serilog.ILogger logger, IConfiguration configuration)
        {
            try
            {
                // Configure the MongoDb data source
                MongoDbSetup mongoDbSetup = new MongoDbSetup(logger, configuration);
                var mongoConnection = configuration.GetMongoConnectionString();
                var databaseName = configuration.GetMongoDatabaseName();
                await mongoDbSetup.InitializeAsync(mongoConnection, databaseName);

            }
            catch (Exception exception)
            {
                logger.Error(exception, $"Unable to initialize the {configuration.GetServiceName()} service.");
                throw;
            }
        }

        /// <summary>
        /// Get's the uptime of the web host.
        /// </summary>
        internal static TimeSpan Uptime
        {
            get
            {
                return (Program._startTime.HasValue) ? DateTime.UtcNow - Program._startTime.Value : new TimeSpan(0, 0, 0);
            }
        }
    }
}
