using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace Mvc3ToolsUpdateWeb_Default.Models
{

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name required")]
        [Display(Name = "User Name")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Email required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        
        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Error : Confirm password does not match with password")]
        public string ConfirmPassword { get; set; }
    }

    public class CustomSerializeModel
    {
        public int UserId { get; set; }
        public List<string> RoleName { get; set; }

    }
}
