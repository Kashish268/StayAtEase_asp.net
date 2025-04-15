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
    }
}
