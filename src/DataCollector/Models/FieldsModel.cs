using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataCollector.Models
{
    public class FieldsModel
    {
        public int ID { get; set; }

        public int? Class_ID { get; set; }

        [Required(ErrorMessage = "نام ستون لازم  است")]
        [DataType(DataType.Text)]
        public string FIELD_Name { get; set; }

        [Required(ErrorMessage = "عنوان ستون لازم  است")]
        [DataType(DataType.Text)]
        public string FIELD_Caption { get; set; }

        [StringLength(150)]
        public string DEF_VAL { get; set; }
        public string FIELD_Type { get; set; }

        public int? Domain_ID { get; set; }

        public double? MAX_VALUE { get; set; }

        public double? MIN_VALUE { get; set; }
        public int? ORDER { get; set; }
        public bool? REQUIERD { get; set; }
    }
}