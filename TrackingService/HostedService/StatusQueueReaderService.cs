using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logger;
using Logger.Models;
using TrackingService.MessageBroker;
using TrackingService.Models;
using TrackingService.Models.RabbitMQ;
using TrackingService.Operations;

namespace TrackingService.HostedService
{
    public class StatusQueueReaderService : HostedService
    {
        private readonly ILog _logManager;
        private readonly IMessageBroker _messageBroker;
        private readonly MessageBrokerOptions _options;
        private readonly IOperation _Operation;

        public StatusQueueReaderService(ILog logManager, IMessageBroker messageBroker, IOperation Operation, IOptions<MessageBrokerOptions> options)
        {
            _logManager = logManager;
            _messageBroker = messageBroker;
            _options = options.Value;
            _Operation = Operation;
        }

        public override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _logManager.LogInfoAsync(new LogModel
                {
                    Message = $"Status Tracker service called !",
                    EventType = EventType.Initiated
                }, stoppingToken);

                // get data from rabbitMQ 

                var subscriptionInfo = await subscribeToMessageBroker<StatusMessageModel>(stoppingToken);


                // process data and update 

                subscriptionInfo.OnReceivedAsync += _Operation.DoOperation;


                var dat = subscriptionInfo.OnErrorAsync += async (message, ex, CancellationToken) =>
                {
                    Console.WriteLine(ex?.Message);
                    await _logManager.LogInfoAsync(new LogModel
                    {
                        Message = ex?.Message,
                        EventType = EventType.Error
                    }, stoppingToken);
                    await Task.Delay(1000);
                };
                await _messageBroker.ProcessMessagesAsync(subscriptionInfo, stoppingToken);

            }
            catch (TaskCanceledException ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = ex.Message,
                    EventType = EventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, stoppingToken);

            }
            catch (OperationCanceledException ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = ex.Message,
                    EventType = EventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                await _logManager.LogErrorAsync(new LogModel
                {
                    Message = ex.Message,
                    EventType = EventType.Exception,
                    LogCategory = LogType.Critical,
                    HttpStatusCode = 500
                }, stoppingToken);
            }

        }

        private Task<SubscriptionInfoModel<T>> subscribeToMessageBroker<T>(CancellationToken stoppingToken) where T : BaseMessageModel
        {
            return Task.FromResult
            (
                new SubscriptionInfoModel<T>()
                {
                    Exchange = _options.AMQPDequeueExchange,
                    Queue = _options.AMQPDequeueQueue,
                    RoutingKey = _options.AMQPDequeueRoute,
                    Requeue = false, //temporary
                }
            );
        }
    }
}
