using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataCollector.Models.Map
{
    public class MapConfigViewModel
    {
        public double MapDefCentroidX { get; set; }
        public double MapDefCentroidY { get; set; }
        public double MapDefultZoom { get; set; }
        public double MapSnapTolerance { get; set; }
    }
}