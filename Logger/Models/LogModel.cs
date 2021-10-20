using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Models
{
    public class LogModel
    {
        public LogType LogCategory { get; set; } = LogType.Information;

        public EventType EventType { get; set; }

        public string Message { get; set; }

        public int? HttpStatusCode { get; set; }

        public string Source { get; set; }

        public string ExpenseId { get; set; }

        public string Service { get; set; }
    }

    public enum LogType
    {
        Information,
        Error,
        Critical,
        Warning
    }

    public enum EventType
    {
        Initiated,
        ReadQueueMessage,
        ClientConnection,
        DatabaseInsert,
        DatabaseInsertFailure,
        DatabaseRead,
        DatabaseReadFailure,
        DatabaseUpdate,
        DatabaseUpdateFailure,
        Exception,
        Error,
        Completed
    }
}
