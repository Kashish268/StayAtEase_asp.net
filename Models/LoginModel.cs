using System.ComponentModel.DataAnnotations;

public class LoginModel
{
    [Required(ErrorMessage = "Email address is required.")]
   
    public string email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string password { get; set; }
}
