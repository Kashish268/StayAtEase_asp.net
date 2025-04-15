namespace WebApplication1.Models
{
    public class MyListingsViewModel
    {
        public List<PropertyViewModel> Properties { get; set; } = new();

        // Summary statistics (auto-calculated)
        public int TotalProperties => Properties?.Count ?? 0;
        public int ActiveListings => Properties?.Count(p => p.IsAvailable) ?? 0;
        public int TotalInquiries { get; set; } = 0;

        // Search and filter
        public string SearchTerm { get; set; } = string.Empty;
        public string Filter { get; set; } = "all";

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalPages => (int)Math.Ceiling((double)TotalProperties / PageSize);
    }


}


