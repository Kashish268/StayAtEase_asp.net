using System.ComponentModel.DataAnnotations;

//public class RegisterViewModel
//{
//    [Required(ErrorMessage = "Full name is required.")]
//    [StringLength(100)]
//    public string FullName { get; set; }

//    [Required(ErrorMessage = "Email address is required.")]
//    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
//    [RegularExpression(@"^[^\d][a-zA-Z0-9._%+-]+@[a-zA-Z]+\.[a-zA-Z]{2,}$",
//        ErrorMessage = "Email should not start with a number.")]
//    [StringLength(150)]
//    public string Email { get; set; }

//    [Required(ErrorMessage = "Phone number is required.")]
//    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
//    [StringLength(15)]
//    public string Phone { get; set; }

//    [Required(ErrorMessage = "Password is required.")]
//    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
//    [StringLength(255)]
//    public string Password { get; set; }

//    [Required(ErrorMessage = "Please confirm your password.")]
//    [Compare("Password", ErrorMessage = "Passwords do not match.")]
//    public string ConfirmPassword { get; set; }

//    [Required(ErrorMessage = "User type is required.")]
//    [StringLength(10)]
//    public string UserType { get; set; }

//    // This property isn't stored in DB but required for form
//    [Required(ErrorMessage = "You must agree to the terms.")]
//    public bool TermsAccepted { get; set; }
//}


using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
public class RegisterViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    public string fullname { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    //[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    //[RegularExpression(@"^[^\d][a-zA-Z0-9._%+-]+@[a-zA-Z]+\.[a-zA-Z]{2,}$",
        //ErrorMessage = "Email should not start with a number.")]
    public string email { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid 10-digit phone number.")]
    public string phone { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string password { get; set; }

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare("password", ErrorMessage = "Passwords do not match.")]
    public string confirmPassword { get; set; }

    [Required(ErrorMessage = "User type is required.")]
    public string userType { get; set; }

    //// ✅ New Optional Field for Profile Picture Upload
    //public IFormFile? profilePicture { get; set; }
}

