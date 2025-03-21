namespace WebApplication1.Models
{
    public class MyListingsViewModel
    {
        public int TotalProperties { get; set; }
        public int ActiveListings { get; set; }
        public int TotalInquiries { get; set; }
        public string? SearchTerm { get; set; }
        public List<PropertyViewModel> Properties { get; set; } = new();
    }

}
