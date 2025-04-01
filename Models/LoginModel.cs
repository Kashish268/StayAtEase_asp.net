using System.ComponentModel.DataAnnotations;

public class LoginModel
{
    [Required(ErrorMessage = "Phone number is required")]
    [Display(Name = "Phone Number")]
    [RegularExpression(@"^[\d\s\+\-\(\)]{10,15}$", ErrorMessage = "Please enter a valid phone number")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }
}
