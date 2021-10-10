using System;
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;

namespace SerilogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(new JsonFormatter())
                .Enrich.FromLogContext()
                .CreateLogger();

            var services = new ServiceCollection();

            services.AddLogging(logging =>
            {
                logging.AddSerilog();
            });
            services.AddSingleton<Foo>();

            var serviceProvider = services.BuildServiceProvider();
            var foo = serviceProvider.GetService<Foo>();
            foo.Run();
        }

        class Foo
        {
            private readonly ILogger<Foo> _logger;
            private readonly IServiceProvider _provider;

            public Foo(ILogger<Foo> logger, IServiceProvider provider)
            {
                _logger = logger;
                _provider = provider;
            }

            public void Run()
            {
                _logger.LogInformation("Run");

                using var ctx1 = LogContext.PushProperty("abc", "a");
                _logger.LogInformation("Run2");
            }
        }
    }
}