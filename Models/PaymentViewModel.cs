namespace WebApplication1.Models
{

    public class PaymentViewModel
    {
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; }
        public string PropertyLocation { get; set; }
        public decimal RentAmount { get; set; }
        //public decimal TaxAmount => RentAmount * 0.18m;
        //public decimal TotalAmount => RentAmount + TaxAmount;
    }
}
