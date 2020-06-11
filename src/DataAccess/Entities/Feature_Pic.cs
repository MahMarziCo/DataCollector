namespace Mah.DataCollector.Entity.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Feature_Pic
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Class_name { get; set; }

        public int? Feature_ID { get; set; }

        [StringLength(50)]
        public string Name_Of { get; set; }

        [StringLength(50)]
        public string Save_ID { get; set; }

        [StringLength(50)]
        public string USER_NAME { get; set; }

        public DateTime? DateOf { get; set; }
    }
}
