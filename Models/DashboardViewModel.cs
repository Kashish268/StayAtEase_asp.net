namespace WebApplication1.Models
{
    public class DashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int ActiveListings { get; set; }
        public int TotalInquiries { get; set; }
        public double ReviewRating { get; set; }
        public List<PropertyMessageViewModel> Messages { get; set; } = new List<PropertyMessageViewModel>();
        public List<PropertyReview> Reviews { get; set; } = new List<PropertyReview>();
    }

    
}
