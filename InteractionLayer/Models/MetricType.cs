using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteractionLayer.Models
{
    public class MetricType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int ClassId { get; set; }
        public int UnitId { get; set; }
        public string Unit { get; set; }
    }
}