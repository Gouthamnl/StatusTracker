using Microsoft.Extensions.DependencyInjection;
using Logger;
using Logger.Serialization;
using TrackingService.DAL.Context;
using TrackingService.DAL.Repository;
using TrackingService.HostedService;
using TrackingService.MessageBroker;
using TrackingService.Operations;

namespace TrackingService.ServiceExtensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBroker, RabbitMQBroker>();
            services.AddScoped<IOperation, StatusUpdateOperation>();
            services.AddTransient<IDbContext, DbContext>();
            services.AddTransient<IStatusTrackerRepository, StatusTrackerRepository>();
            services.AddTransient<IConnectionManager, ConnectionManager>();
            services.AddScoped<ILog, Log>();
            services.AddScoped<ISerializationManager, JsonSerializationManager>();
            services.AddHostedService<StatusQueueReaderService>();
            return services;
        }
    }
}
