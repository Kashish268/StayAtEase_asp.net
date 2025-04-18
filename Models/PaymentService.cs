using Razorpay.Api;

public class PaymentService
{
    private readonly IConfiguration _config;

    public PaymentService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateRentOrder(int propertyId, decimal rentAmount)
    {
        var client = new RazorpayClient(
            _config["Razorpay:KeyId"],
            _config["Razorpay:KeySecret"]);

        var options = new Dictionary<string, object>
        {
            { "amount", rentAmount * 100 }, // Razorpay uses paise
            { "currency", "INR" },
            { "receipt", $"rent_{propertyId}_{DateTime.Now.Ticks}" },
            { "payment_capture", 1 } // Auto-capture payments
        };

        var order = client.Order.Create(options);
        return order["id"].ToString();
    }
}