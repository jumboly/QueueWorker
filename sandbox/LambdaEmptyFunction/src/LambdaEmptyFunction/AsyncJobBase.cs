using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LambdaEmptyFunction
{
    public abstract class AsyncJobBase
    {
        public Task DispatchAsync(string method, string body)
        {
            var methodInfo = GetType().GetMethod(method);
            var parameterInfo = methodInfo.GetParameters().FirstOrDefault();
            var parameter = JsonSerializer.Deserialize(body, parameterInfo.ParameterType);
            return methodInfo.Invoke(this, new[] { parameter }) as Task;
        }
    }
}