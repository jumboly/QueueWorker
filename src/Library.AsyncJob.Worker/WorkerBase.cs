using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

namespace Library.AsyncJob.Worker
{
    public abstract class WorkerBase<TStartUp>
        where TStartUp : class
    {
        protected IServiceProvider ServiceProvider { get; }
        
        protected WorkerBase()
        {
            var services = new ServiceCollection();

            Log.Logger = CreateLogger();
            services.AddLogging(logging => logging.AddSerilog(Log.Logger));

            services.AddSingleton<IConfiguration>(CreateConfiguration());
            services.AddSingleton<IWorkerSettings, WorkerSettings>();
            services.AddSingleton<TStartUp>();

            ServiceProvider = services.BuildServiceProvider();

            var startUp = ServiceProvider.GetService<TStartUp>();
            var configureServices = typeof(TStartUp).GetMethod("ConfigureServices");
            configureServices?.Invoke(startUp, new[] { services });

            ServiceProvider = services.BuildServiceProvider();
        }

        protected async Task DispatchAsync(string message)
        {
            var reader = new StringReader(message);
            var interfaceName = reader.ReadLine()?.Trim();
            var methodName = reader.ReadLine()?.Trim();
            var parameterBody = reader.ReadToEnd();

            var consumerResolver = ServiceProvider.GetService<ConsumerResolver>();
            var consumerType = consumerResolver.Get(interfaceName);

            using (var scope = ServiceProvider.CreateScope())
            {
                var consumer = scope.ServiceProvider.GetService(consumerType) as ConsumerBase;
                await consumer.InvokeAsync(methodName, parameterBody).ConfigureAwait(false);
            }
        }

        private static ILogger CreateLogger()
        {
            var loggerConfiguration = new LoggerConfiguration();
            if (IsDevelopment())
            {
                loggerConfiguration.WriteTo.Console();
            }
            else
            {
                loggerConfiguration.WriteTo.Console(new JsonFormatter());
            }
            loggerConfiguration.Enrich.FromLogContext();

            return loggerConfiguration.CreateLogger();
        }

        private static IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"), true)
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, $"appsettings.{GetEnvironment()}.json"), true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static bool IsDevelopment()
        {
            return string.Equals("Development", GetEnvironment());
        }

        private static string GetEnvironment()
        {
            return new[] { "DOTNETCORE_ENVIRONMENT", "ASPNETCORE_ENVIRONMENT" }
                .Select(Environment.GetEnvironmentVariable)
                .FirstOrDefault(it => it != null);
        }
    }
}