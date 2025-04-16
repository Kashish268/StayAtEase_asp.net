namespace WebApplication1.Models
{
    public class PropertyViewModel
    {
        public int Id { get; set; }                        // PropertyId
        public string Title { get; set; } = string.Empty;  // Title
        public string Location { get; set; } = string.Empty; // You can map it to Address if needed
        public decimal Price { get; set; }                 // Price / Month
        public double Rating { get; set; }                 // Average rating (optional)
        public string ImageUrl { get; set; } = string.Empty; // First image URL
        public int Area { get; set; }                      // SquareFootage
        public string Address { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }              // If needed, else set always true
        public string AddedBy { get; set; } = string.Empty; // User name who added
        public string Status { get; set; } = "Active";     // Active/Inactive
        public int TotalReviews { get; set; }              // Count of reviews
        public int TotalInquiries { get; set; }
        public int TotalProperties { get; set; }
        public int ActiveListings { get; set; }
       
    }
    public class PropertyStatsViewModel
    {
        public int TotalProperties { get; set; }
        public int ActiveListings { get; set; }
        public int TotalInquiries { get; set; }
    }
    public class TotalPropertiesViewModel
    {
        public List<PropertyViewModel> Properties { get; set; }
        public PropertyStatsViewModel Stats { get; set; }
    }

}
