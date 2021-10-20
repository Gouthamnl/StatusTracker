using System;

namespace TrackingService.Models.RabbitMQ
{
    public class BaseMessageModel
    {
        public string CorrelationId { get; set; }

        public string Identity { get; set; }

        public string Exchange { get; set; }

        public string Queue { get; set; }

        public string RoutingKey { get; set; }

        public int RetryCount { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
