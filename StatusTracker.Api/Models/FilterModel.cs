using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatusTracker.Api.Models
{
    public class FilterModel
    {
        public long CompanyId { get; set; } = 0;

        public string ReportId { get; set; }

        public string ExpenseId { get; set; }

        public DateTime from { get; set; }

        public DateTime to { get; set; }
    }
}
