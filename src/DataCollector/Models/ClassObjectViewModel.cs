using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataCollector.Models
{
    public class ClassObjectViewModel
    {
        public string geom { get; set; }
        public int objectid { get; set; }
        public ClassObjectViewModel(int pId, string pGeom)
        {
            geom = pGeom;
            objectid = pId;
        }
    }
}