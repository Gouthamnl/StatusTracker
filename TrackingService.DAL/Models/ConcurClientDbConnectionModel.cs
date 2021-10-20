using System;
using System.Collections.Generic;
using System.Text;

namespace TrackingService.DAL.Models
{
    public class ConcurClientDbConnectionModel
    {
        public long ID { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
    }
}
