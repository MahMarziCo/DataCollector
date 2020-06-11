namespace Mah.DataCollector.Entity.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Script.Serialization;

    [Table("DomainValue")]
    public partial class DomainValue
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Value { get; set; }

        public int? Domain_ID { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        public virtual Domain Domain { get; set; }
    }
}
