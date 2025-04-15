namespace WebApplication1.Models
{
    public class ProfileDetailsViewModel
    {
        public int UserId { get; set; } // Needed to identify the user for updates

        public string ProfileImageUrl { get; set; } = "/images/default-profile.png";

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Timezone { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        // For password update
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
