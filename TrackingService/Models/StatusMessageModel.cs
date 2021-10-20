using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TrackingService.Models.RabbitMQ;

namespace TrackingService.Models
{
    public class StatusMessageModel : BaseMessageModel
    {
        public long CompanyId { get; set; }
        public string TrackingId { get; set; }
        public string ReportId { get; set; }
        public string ExpenseId { get; set; }
        public string Service { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ProcessedDate { get; set; }
        public string Message { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
    }
}
