using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaEmptyFunction
{
    public abstract class LambdaEntryPoint<TStartUp>
        where TStartUp : class
    {
        private IServiceProvider _serviceProvider;
        
        protected LambdaEntryPoint()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, $"appsettings.{Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT")}.json"), true)
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"), true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            
            services.AddSingleton<TStartUp>();
            services.AddSingleton<IConfiguration>(configuration);

            _serviceProvider = services.BuildServiceProvider();
            
            var startUp = _serviceProvider.GetService<TStartUp>();
            var configureServices = typeof(TStartUp).GetMethod("ConfigureServices");
            configureServices?.Invoke(startUp, new[] { services });

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(new JsonFormatter())
                .CreateLogger();
        }
        
        /// <summary>
        /// A simple function that takes a string and returns both the upper and lower case version of the string.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            var body = sqsEvent.Records.FirstOrDefault()?.Body;
            var split = body.Split('\t');

            var typeName = split[0];
            var methodName = split[1];
            var parameter = split[2];
            
            var appSettings = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json"));
            Log.Information("{appSettings}", appSettings);

            await Task.Yield();
            Log.Information("{@event}", sqsEvent, context);
            Log.Information("{@context}", context);
        }
    }

    public record Casing(string Lower, string Upper);
}