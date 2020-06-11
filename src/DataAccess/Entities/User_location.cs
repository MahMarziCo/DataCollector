namespace Mah.DataCollector.Entity.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User_location
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string User_name { get; set; }

        public DbGeometry Coordinate { get; set; }

        public DateTime? DateTime { get; set; }
    }
}
