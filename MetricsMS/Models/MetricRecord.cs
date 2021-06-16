using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetricsMS.Models
{
    public class MetricRecord
    {
        public int UserId { get; set; }
        public int MetricClass { get; set; }
        public int MetricType { get; set; }
        public double Quantity { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class MetricRecordString
    {
        public string MetricClass { get; set; }
        public string MetricType { get; set; }
        public double Measurement { get; set; }
        public DateTime Timestamp { get; set; }
    }
}