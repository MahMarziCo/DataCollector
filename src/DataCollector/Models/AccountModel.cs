using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataCollector.Models
{
    public class AccountModel
    {
        [Required(ErrorMessage="نام کاربری لازم  است")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required(ErrorMessage="کلمه عبور لازم است")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsPersistent { get; set; }


        [HiddenInput]
        public string ReturnUrl { get; set; }

    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "نام کاربری لازم  است")]
        [DataType(DataType.Text)]
        [RegularExpression("^[a-zA-Z0-9._-]*$", ErrorMessage = "در نام کاربری فقط حروف، اعداد و نقطه، خط تیره و خط زیر باید استفاده شود")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "کلمه عبور لازم است")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "تکرار کلمه عبور جدید لازم است")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "تکرار کلمه عبور یکسان نیست")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "نقش کاربر لازم است")]
        [DataType(DataType.Text)]
        public string UserRole { get; set; }
    }

    public class ChangePasswordModel
    {
        public string UserName { get; set; }

        [Required(ErrorMessage = "کلمه عبور قدیم لازم است")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "کلمه عبور جدید لازم است")]
        [DataType(DataType.Password)]

        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تکرار کلمه عبور جدید لازم است")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "تکرار کلمه عبور یکسان نیست")]
        public string ConfirmPassword { get; set; }
    }
}