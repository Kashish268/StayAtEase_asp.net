using System.ComponentModel.DataAnnotations;

public class ProfileDetailsViewModel
{
    public int UserId { get; set; }

    // Default image path if user doesn't upload one
    public string ProfileImage { get; set; } = "/images/default-profile.png";

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; }


    public string Email { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be 10 digits")]
    public string Phone { get; set; }

    // These fields are now optional and will only be validated when filled
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]

    public string? NewPassword { get; set; }


    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }

    public DateTime CreateAt { get; set; }
}
