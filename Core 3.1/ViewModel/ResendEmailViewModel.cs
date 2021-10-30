using System.ComponentModel.DataAnnotations;

namespace SLE_System.Models
{
    public class ResendEmailViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
