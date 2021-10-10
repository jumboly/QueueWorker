using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Library.AsyncJob.Worker
{
    public abstract class ConsoleWorker<TStartUp> : WorkerBase<TStartUp>
        where TStartUp : class
    {
        private readonly IWorkerSettings _settings;
        
        protected ConsoleWorker()
        {
            _settings = ServiceProvider.GetService<IWorkerSettings>();
        }
        
        protected async Task RunAsync()
        {
            try
            {
                var cancel = CreateCancel();

                await ReceiveAsync(cancel.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "異常終了");
                throw;
            }
        }

        private CancellationTokenSource CreateCancel()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, args) =>
            {
                Log.Information("Press Ctrl+C...");
                cancellationTokenSource.Cancel();
            };
            return cancellationTokenSource;
        }

        private async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            var sqs = new AmazonSQSClient();

            try
            {
                var request = new ReceiveMessageRequest()
                {
                    QueueUrl = _settings.QueueUrl,
                    WaitTimeSeconds = 20,
                    MaxNumberOfMessages = _settings.MaxNumberOfMessages,
                    VisibilityTimeout = _settings.VisibilityTimeout
                };
                
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    var response = await sqs.ReceiveMessageAsync(request, cancellationToken).ConfigureAwait(false);
                    
                    foreach (var message in response.Messages)
                    {
                        await DispatchAsync(message.Body).ConfigureAwait(false);

                        // ReSharper disable once MethodSupportsCancellation
                        await sqs.DeleteMessageAsync(new DeleteMessageRequest(_settings.QueueUrl, message.ReceiptHandle))
                            .ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("cancel");
            }
        }
    }
}
