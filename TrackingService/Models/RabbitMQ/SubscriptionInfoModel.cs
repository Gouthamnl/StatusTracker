using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrackingService.Models.RabbitMQ
{
    public class SubscriptionInfoModel<T> where T : BaseMessageModel
    {
        public string Exchange { get; set; }

        public string Queue { get; set; }

        public string RoutingKey { get; set; }

        public bool Requeue { get; set; }

        public Func<T, CancellationToken, Task> OnReceivedAsync { get; set; }

        public Func<T, Exception, CancellationToken, Task> OnErrorAsync { get; set; }
    }
}
