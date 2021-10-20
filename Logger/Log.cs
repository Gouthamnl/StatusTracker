using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Logger.Models;
using Logger.Serialization;

namespace Logger
{
    public interface ILog
    {
        Task LogInfoAsync<T>(T obj, CancellationToken cancellationToken);
        Task LogErrorAsync<T>(T obj, CancellationToken cancellationToken);
        Task LogInfoAsync<T>(T obj);
        Task LogErrorAsync<T>(T obj);
    }

    public class Log : ILog
    {
        private readonly ISerializationManager _serializationManager;
        private readonly LogOptions _configurableOptions;
        public Serilog.Core.Logger _logger;

        public Log(ISerializationManager serializationManager, IOptions<LogOptions> configurableOptions)
        {
            _serializationManager = serializationManager;
            _configurableOptions = configurableOptions.Value;
            _logger = new LoggerConfiguration()
                        .WriteTo.Console(new JsonFormatter())
                        .CreateLogger();
        }
        public async Task LogInfoAsync<T>(T obj, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Run(() =>_logger.Information<T>("TBI Service {@data}", obj));

            }
            catch (Exception ex)
            {
                var f = ex;
            }
        }

        public async Task LogErrorAsync<T>(T obj, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Run(() => _logger.Error<T>("TBI Service {@data}", obj));
            }
            catch (Exception ex)
            {
                var f = ex;
            }
        }

        public async Task LogInfoAsync<T>(T obj)
        {
            try
            {
                await Task.Run(() => _logger.Information<T>("TBI Service {@data}", obj));
            }
            catch (Exception ex)
            {
                var f = ex;
            }
        }

        public async Task LogErrorAsync<T>(T obj)
        {
            try
            {
                await Task.Run(() => _logger.Error<T>("TBI Service {@data}", obj));
            }
            catch (Exception ex)
            {
                var f = ex;
            }
        }
    }
}
