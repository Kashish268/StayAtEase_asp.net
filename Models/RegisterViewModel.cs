using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Verification code is required.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit verification code.")]
    public string VerificationCode { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [RegularExpression(@"^[^\d][a-zA-Z0-9._%+-]+@[a-zA-Z]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Email should not start with a number.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "User type is required.")]
    public string UserType { get; set; }

    [Required(ErrorMessage = "You must agree to the terms.")]
    public bool TermsAccepted { get; set; }
}
