using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Phone number is requied")]
        [RegularExpression(@"^\d{10}$", ErrorMessage ="Please enter valid 10 digit number")]
        public string Phone { get; set; }
    }
}
