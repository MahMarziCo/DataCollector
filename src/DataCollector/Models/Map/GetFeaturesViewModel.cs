using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mah.DataCollector.Web.Models.Map
{
    public class GetFeaturesViewModel
    {
        public IEnumerable<GetFeaturesClassViewModel> LayerNames { get; set; }
        public int MapWidth { get; set; }

    }

    public class GetFeaturesClassViewModel
    {
        public string LayerName { get; set; }
        public string UniqueField { get; set; }
        public double[] MapExtent { get; set; }
        public int SRID { get; set; }
    }
}