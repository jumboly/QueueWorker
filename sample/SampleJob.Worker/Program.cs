using System.Threading.Tasks;
using Library.AsyncJob.Worker;

namespace SampleJob.Worker
{
    class Program : ConsoleWorker<StartUp>
    {
        static Task Main(string[] args)
        {
            return new Program().RunAsync();
        }
    }
}