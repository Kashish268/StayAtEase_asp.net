//using Microsoft.EntityFrameworkCore.Metadata.Internal;

//namespace WebApplication1.Models
//{
//    public class Payments
//    {

//        public int PaymentId { get; set; }
//        public string OrderId { get; set; }
//        public string PaymentGateway { get; set; } = "Razorpay";
//        public decimal Amount { get; set; }
//        public string PaymentStatus { get; set; } = "Pending";
//        public string PaymentMode { get; set; }
//        public string RazorpayPaymentId { get; set; }
//        public string RazorpaySignature { get; set; }
//        public DateTime PaymentDate { get; set; } = DateTime.Now;

//        // Foreign key to the Property table
//        public int PropertyId { get; set; }
//        public Property Property { get; set; }  // Navigation property for the Property table

//        public int UserId { get; set; }
//        public User User { get; set; }  // Navigation property for the User table
//    }
//}
