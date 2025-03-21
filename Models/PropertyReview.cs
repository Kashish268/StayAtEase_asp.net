namespace WebApplication1.Models
{
    public class PropertyReview
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Rating { get; set; }
        public string Review { get; set; }
        public string Image { get; set; }
    }

    public class PropertyReviewViewModel
    {
        public List<PropertyReview> Reviews { get; set; }
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalReviews { get; set; }
    }
}
