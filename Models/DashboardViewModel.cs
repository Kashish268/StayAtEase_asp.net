namespace WebApplication1.Models
{
    public class DashboardViewModel
    {
        /// <summary>
        /// Total number of properties listed by the user.
        /// </summary>
        public int TotalProperties { get; set; }

        /// <summary>
        /// Number of properties currently active.
        /// </summary>
        public int ActiveListings { get; set; }

        /// <summary>
        /// Total number of inquiries received on properties.
        /// </summary>
        public int TotalInquiries { get; set; }

        /// <summary>
        /// Average review rating for the user's properties.
        /// </summary>
        public double ReviewRating { get; set; }

        /// <summary>
        /// Latest inquiry messages for the user's properties.
        /// </summary>
        public List<PropertyMessageViewModel> Messages { get; set; } = new();

        /// <summary>
        /// Recent reviews for the user's properties.
        /// </summary>
        public List<PropertyReview> Reviews { get; set; } = new();

        /// <summary>
        /// Dynamic list of dashboard card info (e.g., icon, label, value).
        /// </summary>
        public List<dynamic> Cards { get; set; } = new();
    }




}
