public class PaymentDisplayViewModel
{
    public int PaymentId { get; set; }
    public string OrderId { get; set; }
    public string PropertyTitle { get; set; }
    public string UserName { get; set; }
    public string PaymentGateway { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
    public string PaymentMode { get; set; }
    public string RazorpayPaymentId { get; set; }
    public string RazorpaySignature { get; set; }
    public DateTime PaymentDate { get; set; }
}
