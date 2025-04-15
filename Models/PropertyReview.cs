namespace WebApplication1.Models
{
    public class PropertyReview
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public string Property { get; set; }
        public string PropertyTitle { get; set; }  // Corrected to Property, matching the SQL query.
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
        public int ReviewsPerPage { get; set; }
    }
}
