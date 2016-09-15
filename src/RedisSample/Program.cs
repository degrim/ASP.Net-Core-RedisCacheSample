using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RedisSample
{
    public class Program
    {
        // Entry point for the application.
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
