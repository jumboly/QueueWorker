using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace Library.AsyncJob.Worker
{
    public static class Extensions
    {
        public static bool IsExtends(this Type type, Type baseType)
        {
            if (type.BaseType == null)
            {
                return false;
            }
            
            if (type.BaseType == baseType)
            {
                return true;
            }

            return IsExtends(type.BaseType, baseType);
        }

        public static IServiceCollection AddConsumer(this IServiceCollection services, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var provider = new ConsumerResolver();
            services.AddSingleton(provider);

            foreach (var consumerType in assembly.GetTypes().Where(t => t.IsExtends(typeof(ConsumerBase))))
            {
                // FooConsumer は IFooJob を実装していること
                
                var interfaceName = $"I{Regex.Replace(consumerType.Name, "Consumer$", "")}Job";
                var interfaceType = consumerType.GetInterfaces().FirstOrDefault(it => it.Name == interfaceName);
                if (interfaceType == null)
                {
                    throw new Exception($"{consumerType.FullName} は {interfaceName} を実装する必要があります");
                }
                
                provider.Add(interfaceType.FullName, consumerType);
                services.AddScoped(consumerType);
            }

            return services;
        }
    }
}