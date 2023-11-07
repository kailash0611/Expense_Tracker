using System.ComponentModel.DataAnnotations;

namespace Authentication.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "Profile Picture")]
        
        public string? ProfilePictureURL { get; set; }

        [Required]
        [DataType(DataType.Password)]
               
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set;}

    }
}
