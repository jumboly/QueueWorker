using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Serilog;

namespace Library.AsyncJob.Worker
{
    public class LambdaWorker<TStartUp> : WorkerBase<TStartUp>
        where TStartUp : class
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext lambdaContext)
        {
            try
            {
                foreach (var record in sqsEvent.Records)
                {
                    Log.Information("受信: messageId={messageId}", record.MessageId);
                    await DispatchAsync(record.Body).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, "異常終了");
                throw;
            }
        }
    }
}