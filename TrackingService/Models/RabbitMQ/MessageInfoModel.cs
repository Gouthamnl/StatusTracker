namespace TrackingService.Models.RabbitMQ
{
    public class MessageInfoModel
    {
        public string Data { get; set; }

        public ulong DeliveryTag { get; set; }
    }
}
