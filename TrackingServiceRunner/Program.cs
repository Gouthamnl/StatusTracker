using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Logger.Models;
using TrackingService.Models;
using TrackingService.DAL.Models;
using TrackingService.ServiceExtensions;

namespace TrackingServiceRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            createHost().Wait();
        }

        static async Task createHost()
        {
            var configuration = new ConfigurationBuilder()
                            .AddEnvironmentVariables()
                            .Build();

            string environment1 = Environment.GetEnvironmentVariable("TRACKING_ENVIRONMENT");
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    services.AddServiceDependencies();
                    services.AddOptions<MessageBrokerOptions>().Configure(options => config.GetSection(nameof(MessageBrokerOptions)).Bind(options));
                    services.AddOptions<DatabaseModel>().Configure(options => config.GetSection(nameof(DatabaseModel)).Bind(options));
                    services.AddOptions<LogOptions>().Configure(options => config.GetSection(nameof(LogOptions)).Bind(options));

                }).ConfigureAppConfiguration((context, builder) =>
                {
                    string environment = Environment.GetEnvironmentVariable("TRACKING_ENVIRONMENT");
                    context.HostingEnvironment.EnvironmentName = environment;

                    builder.SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile($"appSettings.json", true, true);
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
