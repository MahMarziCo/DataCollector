namespace Mah.DataCollector.Entity.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Script.Serialization;

    [Table("Domain")]
    public partial class Domain
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Domain()
        {
            DomainValue = new HashSet<DomainValue>();
            Fields = new HashSet<Fields>();
        }

        public int ID { get; set; }

        [StringLength(50)]
        public string Caption { get; set; }

        public bool? MultiSelect { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ScriptIgnore(ApplyToOverrides =true)]  
        public virtual ICollection<DomainValue> DomainValue { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [ScriptIgnore(ApplyToOverrides = true)]     
        public virtual ICollection<Fields> Fields { get; set; }
    }
}
