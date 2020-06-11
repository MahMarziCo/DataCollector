using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataCollector.Models
{
    public class ClassesModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "نام جدول لازم  است")]
        [DataType(DataType.Text)]
        public string Class_name { get; set; }

        [Required(ErrorMessage = "عنوان فارسی لازم  است")]
        [DataType(DataType.Text)]
        public string Caption { get; set; }

        [Required(ErrorMessage = "نوع لایه لازم  است")]
        [DataType(DataType.Text)]
        public string Class_type { get; set; }

        [Required(ErrorMessage = "سیستم مختصات لازم  است")]
        [DataType(DataType.Text)]
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

        public bool? HasFlow { get; set; }
    }
}