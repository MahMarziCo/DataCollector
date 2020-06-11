using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace Mah.DataCollector.Entity.Entities
{
    public partial class UniqueStyle
    {
        public int ID { get; set; }

        public int? CLASS_ID { get; set; }

        [StringLength(50)]
        public string TEXT { get; set; }

        [StringLength(50)]
        public string FillColor { get; set; }

        [StringLength(50)]
        public string StrokColor { get; set; }

        public double? Width { get; set; }

        public double? StrokWidth { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        public virtual Classes Classes { get; set; }
    }
}
