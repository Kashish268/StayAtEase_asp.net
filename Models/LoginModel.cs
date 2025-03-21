using System.ComponentModel.DataAnnotations;


namespace WebApplication1.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
