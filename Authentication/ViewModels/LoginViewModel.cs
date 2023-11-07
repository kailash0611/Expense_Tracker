using System.ComponentModel.DataAnnotations;

namespace Authentication.ViewModel
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Display(Name = "Email Address")]
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set;}

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}