using WebApplication1.Models;

public class SuperAdminDashboardViewModel
{
    public int TotalProperties { get; set; }
    public int ActiveListings { get; set; }
    public int TotalInquiries { get; set; }
    public double AverageReviewRating { get; set; }
   
    public List<PropertyViewModel> Properties { get; set; }
    public List<User> Tenants { get; set; } = new();
    public List<User> PropertyOwners { get; set; } = new();
    public List<PropertyMessageViewModel> LatestMessages { get; set; } = new();
    public List<PropertyReview> LatestReviews { get; set; } = new();  // Latest Reviews
}

