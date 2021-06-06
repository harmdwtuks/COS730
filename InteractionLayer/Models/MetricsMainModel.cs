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

    public class NewTypeAndClassViewModel
    {
        public List<MetricClass> MetricCasses { get; set; }
        public List<MetricUnits> MetricUnits { get; set; }
        public List<MetricType> CurrentTypes { get; set; }
    }

    public class NewType
    {
        public int MetricClassId { get; set; }
        public string MetricType { get; set; }
        public int MetricUnitId { get; set; }
    }
}