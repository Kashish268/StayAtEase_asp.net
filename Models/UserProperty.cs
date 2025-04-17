namespace WebApplication1.Models
{
    public class UserProperty
    {
        public int PropertyID { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public int Beds { get; set; }
        public int Baths { get; set; }
        public string Size { get; set; }
        public string Price { get; set; }
        public string ImagePath { get; set; }
        public decimal Rating { get; set; }
        public int Reviews { get; set; }

        public string PropertyType { get; set; }

        public string CreatedAt { get; set; }

        public bool IsWishlisted { get; set; }

        public string RatingDisplay => Rating > 0 ? Rating.ToString("0.0") : "N/A";


    }
}
