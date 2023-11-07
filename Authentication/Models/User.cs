using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class User : IdentityUser
    {
      
       public string? FirstName { get; set; }
        
        public string? LastName { get; set; }

        public string? ProfilePictureURL { get; set; }
    }
}
