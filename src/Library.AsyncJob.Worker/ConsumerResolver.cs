using System;
using System.Collections.Concurrent;

namespace Library.AsyncJob.Worker
{
    public class ConsumerResolver
    {
        private readonly ConcurrentDictionary<string, Type> _cache = new();

        public void Add(string interfaceName, Type consumerType)
        {
            if (!_cache.TryAdd(interfaceName, consumerType))
            {
                throw new Exception($"重複しています: {interfaceName}");
            }
        }

        public Type Get(string interfaceName)
        {
            if (!_cache.TryGetValue(interfaceName, out var consumerType))
            {
                throw new Exception($"登録されていません: {interfaceName}");
            }
            return consumerType;
        }
    }
}