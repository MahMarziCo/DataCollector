namespace Mah.DataCollector.Entity.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Script.Serialization;

    public partial class Classes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Classes()
        {
            Fields = new HashSet<Fields>();
            UniqueStyles = new HashSet<UniqueStyle>();
        }

        public int ID { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "نام جدول لازم  است")]
        public string Class_name { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "عنوان فارسی لازم  است")]
        public string Caption { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "نوع لایه لازم  است")]
        public string Class_type { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "سیستم مختصات لازم  است")]
        public string SpatialRefrence { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "عدد صحیح بزرگتر از 1 باید وارد شود")]
        public double? Scale { get; set; }

        [StringLength(50)]
        public string FillColor { get; set; }

        [StringLength(50)]
        public string StrokColor { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "عدد صحیح بزرگتر از 0 باید وارد شود")]
        public double? Width { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "عدد صحیح بزرگتر از 0 باید وارد شود")]
        public double? StrokWidth { get; set; }

        [StringLength(50)]
        public string UniqueField { get; set; }
        
        [StringLength(50)]
        public string AdressField { get; set; }
        
        [StringLength(50)]
        public string DateOf { get; set; }

        [StringLength(50)]
        public string SupervisorField { get; set; }

        [StringLength(50)]
        public string SupervisorDateOfField { get; set; }
        
        [StringLength(50)]
        public string UserId { get; set; }
        
        public int? ClassOrder { get; set; }
        public bool? HasFlow { get; set; }


        [ScriptIgnore(ApplyToOverrides = true)]
        public virtual ICollection<Fields> Fields { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        
        public virtual ICollection<UniqueStyle> UniqueStyles { get; set; }


    }
}
