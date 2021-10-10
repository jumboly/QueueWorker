using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LambdaEmptyFunction
{
    public class AsyncJob : AsyncJobBase
    {
        private readonly ILogger<AsyncJob> _logger;

        public AsyncJob(ILogger<AsyncJob> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(Parameter parameter)
        {
            _logger.LogInformation("{@parameter}", parameter);
        }
    }
}