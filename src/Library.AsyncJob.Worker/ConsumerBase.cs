using System.Text.Json;
using System.Threading.Tasks;

namespace Library.AsyncJob.Worker
{
    public abstract class ConsumerBase
    {
        public async Task InvokeAsync(string methodName, string parameterBody)
        {
            var method = GetType().GetMethod(methodName);
            var parameterType = method.GetParameters()[0].ParameterType;
            var parameter = JsonSerializer.Deserialize(parameterBody, parameterType);
            var task = method.Invoke(this, new[] { parameter }) as Task;
            await task.ConfigureAwait(false);
        }
    }
}