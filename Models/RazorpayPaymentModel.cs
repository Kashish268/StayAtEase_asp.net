using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Models
{
    public class RazorpayPaymentModel
    {
        public string RazorpayPaymentId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpaySignature { get; set; }
        public int UserId { get; set; }
        public string PropertyId { get; set; }
        public decimal Amount { get; set; }
    }
}
