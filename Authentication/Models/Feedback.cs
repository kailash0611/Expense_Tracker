
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required(ErrorMessage = "Please Enter the Details")]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter the Details")]
        [Column(TypeName = "nvarchar(30)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter the Details")]
        [Column(TypeName = "nvarchar(200)")]
        public string Suggestion { get; set; }

        public string? UserId { get; set; }
        public User? user { get; set; }
    }
}
