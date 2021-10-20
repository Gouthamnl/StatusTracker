using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logger.Serialization;
using TrackingService.Exceptions;
using TrackingService.Models;
using TrackingService.Models.RabbitMQ;

namespace TrackingService.MessageBroker
{
    public interface IMessageBroker
    {
        Task EnqueueAsync<T>(T obj, CancellationToken cancellationToken) where T : BaseMessageModel;

        Task ProcessMessagesAsync<T>(SubscriptionInfoModel<T> subscriptionInfo, CancellationToken cancellationToken) where T : BaseMessageModel;
    }

    public class RabbitMQBroker : IMessageBroker
    {
        private const string ConnectionUnavailable = "Connection unavailable";

        private readonly ISerializationManager serializationManager;
        private readonly MessageBrokerOptions messageBrokerOptions;
        private readonly ConnectionFactory connectionFactory;
        private readonly object syncLock;
        private IConnection connection;

        public RabbitMQBroker(ISerializationManager serializationManager, IOptions<MessageBrokerOptions> options)
        {
            if (options?.Value == null)
            {
                throw new ArgumentException(nameof(options));
            }

            this.syncLock = new object();
            this.serializationManager = serializationManager;
            this.messageBrokerOptions = options.Value;
            this.connectionFactory = new ConnectionFactory();
            this.connectionFactory.AutomaticRecoveryEnabled = true;
            this.connectionFactory.UserName = this.messageBrokerOptions.RabbitMqUsername;
            this.connectionFactory.Password = this.messageBrokerOptions.RabbitMqPassword;
            this.connectionFactory.HostName = this.messageBrokerOptions.Host;
        }

        public Task EnqueueAsync<T>(T obj, CancellationToken cancellationToken) where T : BaseMessageModel
        {
            if (!IsConnected)
            {
                TryConnect();
            }

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(obj.Exchange, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
                channel.QueueDeclare(obj.Queue, true, false, false);
                channel.QueueBind(obj.Queue, obj.Exchange, obj.RoutingKey);
                channel.BasicPublish(obj.Exchange, obj.RoutingKey, null, Encoding.UTF8.GetBytes(this.serializationManager.Serialize(obj)));
            }

            return Task.CompletedTask;
        }

        public bool IsConnected
        {
            get
            {
                return this.connection != null && this.connection.IsOpen;
            }
        }

        public async Task ProcessMessagesAsync<T>(SubscriptionInfoModel<T> subscriptionInfo, CancellationToken cancellationToken)
            where T : BaseMessageModel
        {
            if (!IsConnected)
            {
                TryConnect();
            }

            var channel = this.connection.CreateModel();
            channel.ExchangeDeclare(subscriptionInfo.Exchange, ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            channel.QueueDeclare(subscriptionInfo.Queue, true, false, false);
            channel.QueueBind(subscriptionInfo.Queue, subscriptionInfo.Exchange, subscriptionInfo.RoutingKey);
            channel.BasicQos(0, this.messageBrokerOptions.Batch, false);

            var tasks = new List<Task>();
            var queue = new ConcurrentQueue<MessageInfoModel>();
            tasks.Add(SubscribeAsync(channel, queue, subscriptionInfo.Queue));
            for (int i = 0; i < this.messageBrokerOptions.Batch; i++)
            {
                tasks.Add(StartWorkerAsync(channel, queue, subscriptionInfo, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        private Task SubscribeAsync(IModel channel, ConcurrentQueue<MessageInfoModel> queue, string queueName)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                var messageString = Encoding.UTF8.GetString(eventArgs.Body);
                var message = new MessageInfoModel()
                {
                    Data = messageString,
                    DeliveryTag = eventArgs.DeliveryTag
                };

                queue.Enqueue(message);
            };

            channel.BasicConsume(queueName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task StartWorkerAsync<T>(IModel channel, ConcurrentQueue<MessageInfoModel> queue, SubscriptionInfoModel<T> subscriptionInfo, CancellationToken cancellationToken)
            where T : BaseMessageModel
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                MessageInfoModel messageInfo;
                var result = queue.TryDequeue(out messageInfo);
                if (messageInfo != null)
                {
                    var message = this.serializationManager.Deserialize<T>(messageInfo.Data);

                    try
                    {
                        await subscriptionInfo.OnReceivedAsync?.Invoke(message, cancellationToken);
                        channel.BasicAck(messageInfo.DeliveryTag, false);
                    }
                    catch (Exception ex)

                    {
                        if (subscriptionInfo.Requeue)
                        {
                            channel.BasicNack(messageInfo.DeliveryTag, false, true);
                        }

                        await subscriptionInfo?.OnErrorAsync?.Invoke(message, ex, cancellationToken);
                    }
                }
                else
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        private void TryConnect()
        {
            lock (this.syncLock)
            {
                try
                {
                    if (IsConnected)
                    {
                        return;
                    }

                    this.connection = connectionFactory.CreateConnection();
                    if (!IsConnected)
                    {
                        throw new MessageBrokerException(ConnectionUnavailable);
                    }
                }
                catch (BrokerUnreachableException ex)
                {
                    throw new MessageBrokerException(ex.Message);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                    }

                    throw new MessageBrokerException(ex.Message);
                }
            }
        }
    }
}
