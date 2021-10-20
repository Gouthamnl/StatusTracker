namespace TrackingService.Models
{
    public class MessageBrokerOptions
    {
        public string Host { get; set; }

        public string RabbitMqUsername { get; set; }

        public string RabbitMqPassword { get; set; }

        public ushort Batch { get; set; }

        public string AMQPDequeueExchange { get; set; }

        public string AMQPDequeueQueue { get; set; }

        public string AMQPDequeueRoute { get; set; }

    }
}
