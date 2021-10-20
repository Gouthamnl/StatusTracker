using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StatusTracker.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        static void ConfigConfiguration(IConfigurationBuilder config)
        {
            string environment = Environment.GetEnvironmentVariable("TRACKING_ENVIRONMENT");
            config.SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigConfiguration)
                .UseStartup<Startup>();
    }
}
