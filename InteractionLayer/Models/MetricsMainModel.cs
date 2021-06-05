using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InteractionLayer.Models
{
    public class MetricsMainModel
    {
        public int MetricClass { get; set; }
        public int MetricType { get; set; }
        public double Quantity { get; set; }
        public DateTime Timestamp { get; set; }

        public List<SelectListItem> MetricClasses { get; set; }
        //public SelectList MetricClasses { get; set; }
        public List<SelectListItem> MetricTypes { get; set; }
        //public SelectList MetricTypes { get; set; }
    }

    public class MetricsViewMainModel
    {
        public List<string> MetricTypes { get; set; }
        public List<MetricRecord> MetricRecords { get; set; }
    }
}