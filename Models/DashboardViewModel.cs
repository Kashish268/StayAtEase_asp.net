namespace WebApplication1.Models
{
    public class DashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int ActiveListings { get; set; }
        public int TotalInquiries { get; set; }
        public decimal AverageRating { get; set; }
        public List<DashboardMessage> Messages { get; set; }
        public List<LatestReview> Reviews { get; set; }
    }

    public class DashboardMessage
    {
        public string Name { get; set; }
        public string PropertyId { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string MessageText { get; set; }
    }

    public class LatestReview
    {
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public string Date { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public string PropertyTitle { get; set; }
    }





}
