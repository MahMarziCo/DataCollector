using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataCollector.Models
{
    public class UserModel
    {
        public string UserId{ get; set; }

        [Required(ErrorMessage = "نام کاربری لازم  است")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "کلمه عبور لازم  است")]
        [DataType(DataType.Text)]
        public string Pass { get; set; }

        [Required(ErrorMessage = "نقش کاربر را وارد کنید")]
        [DataType(DataType.Text)]
        public string Role { get; set; }
    }
}