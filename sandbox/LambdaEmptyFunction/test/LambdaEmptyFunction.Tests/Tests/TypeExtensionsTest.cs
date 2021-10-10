using System.Reflection;
using Xunit;

namespace LambdaEmptyFunction.Tests.Tests
{
    public class TypeExtensionsTest
    {
        [Fact]
        public void Test()
        {
            Assert.True(typeof(Bar).IsExtends<Foo>());
            Assert.True(typeof(Baz).IsExtends<Bar>());
            Assert.True(typeof(Baz).IsExtends<Foo>());
            Assert.False(typeof(Fuga).IsExtends<Foo>());
            Assert.True(typeof(Fuga).IsExtends<object>());
            Assert.False(typeof(object).IsExtends<object>());
        }

        class Foo
        {
        }

        class Bar : Foo
        {
        }

        class Baz : Bar
        {
        }

        class Hoge
        {
        }

        class Fuga : Hoge
        {
            
        }
    }
}