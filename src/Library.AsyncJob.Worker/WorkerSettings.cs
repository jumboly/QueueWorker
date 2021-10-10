using Microsoft.Extensions.Configuration;

namespace Library.AsyncJob.Worker
{
    public interface IWorkerSettings
    {
        string QueueUrl { get; }
        int MaxNumberOfMessages { get; }
        int VisibilityTimeout { get; }
    }

    public class WorkerSettings : IWorkerSettings
    {
        public WorkerSettings(IConfiguration configuration)
        {
            configuration.GetSection(nameof(WorkerSettings)).Bind(this);
        }
        public string QueueUrl { get; set; }
        public int MaxNumberOfMessages { get; set; } = 1;
        public int VisibilityTimeout { get; set; } = 30;
    }
}