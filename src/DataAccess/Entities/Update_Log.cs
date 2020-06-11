namespace Mah.DataCollector.Entity.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Update_Log
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string User_NAME { get; set; }

        public int? Feature_ID { get; set; }

        [StringLength(50)]
        public string Class_name { get; set; }

        public DateTime? DateTime { get; set; }

         [StringLength(50)]
        public string ActionOf { get; set; }

        [Column(TypeName = "text")]
        public string DataOf { get; set; }
    }
}
