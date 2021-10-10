using Library.AsyncJob.Worker;
using Microsoft.Extensions.DependencyInjection;
using SampleJob.Worker.Services;

namespace SampleJob.Worker
{
    public class StartUp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICalcService, CalcService>();
            
            services.AddConsumer();
        }
    }
}