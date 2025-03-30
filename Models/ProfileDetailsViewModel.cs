namespace WebApplication1.Models
{
    public class ProfileDetailsViewModel
    {
        public string ProfileImageUrl { get; set; } = "/images/default-profile.png";
        public string FirstName { get; set; } = "John";
        public string LastName { get; set; } = "Doe";
        public string Email { get; set; } = "john.doe@example.com";
        public string Phone { get; set; } = "+1 (555) 000-0000";
        public string Location { get; set; } = "San Francisco, CA";
        public string Timezone { get; set; } = "(GMT-08:00) Pacific Time";
        public string Bio { get; set; } = "Property dealer";
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
