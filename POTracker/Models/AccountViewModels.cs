using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.WebPages.Html;

namespace POTracker.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class SupplierOption 
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        //[Required]
        //[Display(Name = "Email")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Please Select Type of roles")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email id")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string Selected { get; set; }
        public List<Roles> SelectRoles { get; set; }
        public IEnumerable<Country> Select_Country { get; set; }
        public string Country { get; set; }
        public string RolesID { get; set; }
        public IEnumerable<SupplierOption> SelectSupplier{get;set;}
        //[Required]
        //[Display(Name = "Select Type of User")]
        //public string User { get; set; }
    }
    public class Country
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class Roles
    {
        public Roles() { }
        public Roles(DataRow row) {
        //    Id = row.Field<string>("Id");
            Name = row.Field<string>("Name");
        }
        public string Name { get; set; }
        public bool Checked { get; set; }

    }
    public class RolesList
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
    public class ShowLoginUser
    {
        //public ShowLoginUser(DataRow row)
        //{
        //    //UserId = row.Field<string>("UserId");
        //    //UserName = row.Field<string>("UserName");
        
           
        //}
       // public ShowLoginUser() { }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }

        public string UserInRole { get; set; }
        public List<ShowLoginUser> UserList { get; set; }
        public List<getListofRoles> RoleList { get; set; }
    }
    public class getListofRoles
    {
        public string Value { get; set; }
        public string Name { get; set; }

    }
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
