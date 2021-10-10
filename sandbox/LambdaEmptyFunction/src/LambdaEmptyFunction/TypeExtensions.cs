using System;

namespace LambdaEmptyFunction
{
    public static class TypeExtensions
    {
        public static bool IsExtends<T>(this Type type) where T : class 
        {
            if (!type.IsClass || type.BaseType == null)
            {
                return false;
            }
            if (type.BaseType == typeof(T))
            {
                return true;
            }
            return type.BaseType.IsExtends<T>();
        }
    }
}