using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DemoSettingsIocLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var logDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Logs\";
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var serviceProvider = new ServiceCollection()
            .AddLogging(logOptions => {
                logOptions.SetMinimumLevel(LogLevel.Trace);

                logOptions.AddFile(logDirectory + "log-{Date}.txt", LogLevel.Debug); // -> File using Serilog
                logOptions.AddConsole(); // -> CMD
                logOptions.AddDebug(); // -> VS Output window
                logOptions.AddEventLog(eventLogOptions => eventLogOptions.SourceName = "DemoSettingsIocLog"); // -> Windows Event Viewer (if you are running a dubug from visual studio, run it as admin)
            })
            .BuildServiceProvider();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Program>();

            var appVersion = config.GetValue<string>("AppVersion");

            logger.LogInformation($"App Version: {appVersion}");
            logger.LogDebug($"Starting application...");

            Console.WriteLine("Hello World!");
            Console.WriteLine($"Log Directory: {logDirectory}");

            logger.LogDebug($"Application started!");
        }
    }
}
