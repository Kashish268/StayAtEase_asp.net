

namespace WebApplication1.Models
{
    public class PropertyDetailsViewModel
    {
        public string PropertyId { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int SquareFootage { get; set; }

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
      
        public string PropertyType { get; set; }

        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerMobile { get; set; }
        public bool IsAvailable { get; set; }

        public double AverageRating { get; set; }
        public List<ReviewModel> Reviews { get; set; }
        public List<InquiryModel> Inquiries { get; set; }

        public string OwnerProfilePic { get; set; }
        public bool IsPaymentDone { get; set; }

        public Review UserReview { get; set; }


    }

    public class ReviewModel
    {
        public int ReviewId { get; set; }
        public string PropertyTitle { get; set; }
        public string ReviewerName { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public string ProfilePic { get; set; }
        

    }


    public class InquiryModel
    {
        public int InquiryId { get; set; }
        public string GuestName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Message { get; set; }
    }





}
