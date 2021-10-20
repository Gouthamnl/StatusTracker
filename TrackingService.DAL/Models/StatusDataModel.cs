using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TrackingService.DAL.Models
{
    public class StatusDataModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public long CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string TrackingId { get; set; }

        public string ReportId { get; set; }

        public string ExpenseId { get; set; }

        public string CurrentService { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DownloadEngine { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DatabaseService { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? RecoveryEngine { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? ImageDownloader { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? PdfSplitter { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? IsOriginal { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? VATValidationService { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? GoogleVisionService { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? ML { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? MLSender { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? MLReceiver { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? RulesEngine { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? Migration { get; set; } = null;

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? AutoAudit { get; set; } = null;

        [BsonIgnore]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? ProcessedDate { get; set; } = null;

        public string Process { get; set; }

        public string Message { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime StartDate { get; set; }

        public string Status { get; set; }

        [BsonIgnore]
        public string CurrentStatus { get; set; }
    }

    public enum ServiceTypes
    {
        DownloadEngine,
        DatabaseService,
        RecoveryEngine,
        ImageDownloader,
        PdfSplitter,
        IsOriginal,
        MLSender,
        MLReceiver,
        ML,
        RulesEngine,
        AutoAudit,
        Migration,
        VATValidationService,
        GoogleVisionService
    }

    public enum ProcessStatus
    {
        InProgress,
        Completed,
        PartiallyCompleted
    }

    public enum Status
    {
        Success,
        Failed
    }
}
