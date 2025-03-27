namespace WebApplication1.Models
{
    public class MyListingsViewModel
    {
        public List<PropertyViewModel> Properties { get; set; } = new List<PropertyViewModel>();
        public int TotalProperties { get; set; }
        public int ActiveListings { get; set; }
        public int TotalInquiries { get; set; }

        // Add SearchTerm
        public string SearchTerm { get; set; } = string.Empty;

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalPages => (int)Math.Ceiling((double)TotalProperties / PageSize);
    }

    }


