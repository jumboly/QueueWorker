using Amazon.Lambda.Core;
using Library.AsyncJob.Worker;

namespace SampleJob.Worker
{
    public class Lambda : LambdaWorker<StartUp>
    {
    }
}