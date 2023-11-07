using System.ComponentModel.DataAnnotations;

namespace Authentication.ViewModels
{
    public class ReportViewModel
    {
        [Required (ErrorMessage = "Days field is required")]
        public int Days { get; set; }
    }
}
