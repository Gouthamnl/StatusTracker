using System.Collections.Generic;
using TrackingService.DAL.Models;

namespace StatusTracker.Api.Models
{
    public class StatusResultModel
    {
        public List<StatusDataModel> StatusDataModel { get; set; }

        public int? Success { get; set; }

        public int? InProgress { get; set; } = 0;

        public int? Failure { get; set; } = 0;

        public long TotalCount { get; set; }
    }
}
