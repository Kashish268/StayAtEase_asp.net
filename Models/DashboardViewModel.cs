namespace WebApplication1.Models
{
    /// <summary>
    /// ViewModel for the Dashboard displaying various statistics and information.
    /// </summary>
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
        /// Defaults to 0 if no reviews are present.
        /// </summary>
        public double ReviewRating { get; set; } = 0.0; // Default to 0 if no reviews exist

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
        public List<DashboardCard> Cards { get; set; } = new();

        /// <summary>
        /// Constructor to initialize the dynamic cards with labels and values.
        /// Automatically called when creating the DashboardViewModel.
        /// </summary>
        public DashboardViewModel()
        {
            InitializeDashboardCards();
        }

        /// <summary>
        /// Initializes the dashboard cards with their respective information.
        /// </summary>
        private void InitializeDashboardCards()
        {
            Cards.Add(new DashboardCard
            {
                Icon = "fas fa-building text-primary",
                Label = "Total Properties",
                Value = TotalProperties.ToString()
            });

            Cards.Add(new DashboardCard
            {
                Icon = "fas fa-check-circle text-success",
                Label = "Active Listings",
                Value = ActiveListings.ToString()
            });

            Cards.Add(new DashboardCard
            {
                Icon = "fas fa-envelope text-warning",
                Label = "Total Inquiries",
                Value = TotalInquiries.ToString()
            });

            Cards.Add(new DashboardCard
            {
                Icon = "fas fa-star text-info",
                Label = "Review Rating",
                // Formatting to 1 decimal place
                Value = ReviewRating.ToString("F1")
            });
        }
    }

    /// <summary>
    /// Represents a dynamic dashboard card to be displayed.
    /// </summary>
    public class DashboardCard
    {
        /// <summary>
        /// Icon class for the card.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Label for the card.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Value to be displayed on the card.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Represents the details of a property message inquiry.
    /// </summary>
   

    /// <summary>
    /// Represents a property review.
    /// </summary>
    public class PropertyReview
    {
        public string ReviewerName { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
