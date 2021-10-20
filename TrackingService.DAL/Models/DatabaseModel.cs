using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingService.DAL.Models
{
    public class DatabaseModel
    {
        public string MongoConnectionString { get; set; }
        public string Database { get; set; }
        public string ConcurConnection { get; set; }
    }
}
