namespace Mah.DataCollector.Entity.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TB_Setting
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string SET_Key { get; set; }

        [Column(TypeName = "text")]
        public string Value { get; set; }

        [StringLength(50)]
        public string User_name { get; set; }

        [StringLength(50)]
        public string SET_Type { get; set; }
    }
}
