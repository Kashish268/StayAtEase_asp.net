namespace WebApplication1.Models
{
    public class PropertyReview
    {
        public int Id { get; set; }  // Ensure Id is an integer
        public string Name { get; set; }
        public string Date { get; set; }
        public int Rating { get; set; }  // Change Rating to an integer
        public string Text { get; set; }
        public string Property { get; set; } 
        public string ImageUrl { get; set; }
 
 

    }

    public class PropertyReviewViewModel
    {
        public List<PropertyReview> Reviews { get; set; }
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Property { get; set; }
        public int TotalReviews { get; set; }
    }
}
