using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models
{
    public class MetricRecord
    {
        public string MetricClass { get; set; }
        public string MetricType { get; set; }
        public double Measurement { get; set; }
        public DateTime Timestamp { get; set; }
    }
}