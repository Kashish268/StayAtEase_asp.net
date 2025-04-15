namespace WebApplication1.Models
{
    public class PropertyViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; } // ✅ Decimal

        public double Rating { get; set; }
        public string ImageUrl { get; set; }

        public int Area { get; set; }  // Ensure this property exists
        public string Address { get; set; } = string.Empty; // Ensure this property exists
        public bool IsAvailable { get; set; } // Ensure this property exists


       
    }
}
