using Microsoft.Extensions.Configuration;

namespace LambdaEmptyFunction
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            configuration.GetSection(nameof(AppSettings)).Bind(this);
        }
        
        public string Foo { get; set; }
    }
}