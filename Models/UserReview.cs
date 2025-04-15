namespace WebApplication1.Models
{
    public class UserReview
    {
        public int PropertyID { get; set; }
        public int UserID { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
