namespace Mah.DataCollector.Entity.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Script.Serialization;

    public partial class Fields
    {
        public int ID { get; set; }

        public int? Class_ID { get; set; }

        [StringLength(50)]
        public string FIELD_Name { get; set; }

        [StringLength(50)]
        public string FIELD_Caption { get; set; }

        [StringLength(150)]
        public string DEF_VAL { get; set; }

        [StringLength(10)]
        public string FIELD_Type { get; set; }

        public int? Domain_ID { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        public virtual Classes Classes { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        public virtual Domain Domain { get; set; }

        public double? MAX_VALUE { get; set; }

        public double? MIN_VALUE { get; set; }
        public int? ORDER { get; set; }
        public bool? REQUIERD { get; set; }
    }
}
