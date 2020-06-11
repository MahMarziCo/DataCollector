using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataCollector.Models
{
    public class UniquStyleModel
    {
        public int ID { get; set; }

        public int? CLASS_ID { get; set; }

        public string TEXT { get; set; }

        public string FillColor { get; set; }

        public string StrokColor { get; set; }

        public double? Width { get; set; }

        public double? StrokWidth { get; set; }

    }
}