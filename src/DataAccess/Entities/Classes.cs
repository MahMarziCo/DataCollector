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

        [Required(ErrorMessage = "نام جدول لازم  است")]
        public string Class_name { get; set; }

        [Required(ErrorMessage = "عنوان فارسی لازم  است")]
        public string Caption { get; set; }

        [Required(ErrorMessage = "نوع لایه لازم  است")]
        public string Class_type { get; set; }

        [Required(ErrorMessage = "سیستم مختصات لازم  است")]
        public string SpatialRefrence { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "عدد صحیح بزرگتر از 1 باید وارد شود")]
        public double? Scale { get; set; }

        public string FillColor { get; set; }

        public string StrokColor { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "عدد صحیح بزرگتر از 0 باید وارد شود")]
        public double? Width { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "عدد صحیح بزرگتر از 0 باید وارد شود")]
        public double? StrokWidth { get; set; }

        public string UniqueField { get; set; }
        
        public string AdressField { get; set; }
        
        public string DateOf { get; set; }
        public string TimeOf { get; set; }

        public string SupervisorField { get; set; }

        public string SupervisorDateOfField { get; set; }
        
        public string UserId { get; set; }
        
        public int? ClassOrder { get; set; }
        public bool? HasFlow { get; set; }

        public  bool? RequieredPhoto { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        public virtual ICollection<Fields> Fields { get; set; }

        public virtual ICollection<UniqueStyle> UniqueStyles { get; set; }
        


    }
}
