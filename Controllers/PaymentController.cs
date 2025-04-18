using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApplication1.Models; // Apne model namespace ka dhyan rakhein
using Razorpay.Api;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApplication1.Models; // Adjust as per your actual namespace
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Filters;

public class PaymentController : Controller
{
    private readonly IConfiguration _configuration;

    public PaymentController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId.HasValue)
        {
            ViewBag.ProfileImage = GetProfileImagePath(userId.Value);
        }

        base.OnActionExecuting(context);
    }

    protected string GetProfileImagePath(int userId)
    {
        string profilePath = null;

        // Shared connection string (update with your actual connection string)
        string connectionString = _configuration.GetConnectionString("DefaultConnection");


        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT profile_pic FROM users WHERE user_id = @UserId";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            conn.Open();
            var result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                profilePath = result.ToString();
            }
        }

        return profilePath;
    }



    public IActionResult Payment(int propertyId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!userId.HasValue)
        {
            ViewBag.IsLoggedIn = false;
            return View();
        }

       

       /* string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = @"SELECT COUNT(*) FROM Payments 
                         WHERE UserId = @UserId AND PropertyId = @PropertyId AND PaymentStatus = 'Success'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId.Value);
                command.Parameters.AddWithValue("@PropertyId", propertyId);

                int count = (int)command.ExecuteScalar();
                isPaymentDone = count > 0;
            }
        }*/

       /* ViewBag.PaymentDone = isPaymentDone;
*/
        // Sirf jab payment nahi hua tabhi details fetch karna
       
        
            var paymentDetails = GetPaymentDetails(userId.Value, propertyId);
            ViewBag.PaymentDetails = paymentDetails;
        

        ViewBag.IsLoggedIn = true;
        return View();
    }

    //public IActionResult Payment(int propertyId)
    //{
    //    var userId = HttpContext.Session.GetInt32("UserId");

    //    if (!userId.HasValue)
    //    {
    //        ViewBag.IsLoggedIn = false;
    //        return View(); // View will show not logged in message
    //    }



    //    //int propertyId = 1; // Hardcoded for now
    //    var paymentDetails = GetPaymentDetails(userId.Value, propertyId);

    //    ViewBag.IsLoggedIn = true;
    //    ViewBag.PaymentDetails = paymentDetails;

    //    return View();
    //}

    private PaymentViewModel GetPaymentDetails(int userId, int propertyId)
    {
        PaymentViewModel model = new PaymentViewModel();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "SELECT PropertyId, Title, Address, Price FROM Properties WHERE PropertyId = @PropertyId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@PropertyId", propertyId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.UserId = userId;
                        model.PropertyId = Convert.ToInt32(reader["PropertyId"]);
                        model.PropertyTitle = reader["Title"].ToString();
                        model.PropertyLocation = reader["Address"].ToString();
                        model.RentAmount = Convert.ToDecimal(reader["Price"]);
                    }
                }
            }
        }

        return model;
    }

    [HttpPost]
    public IActionResult ProceedPayment(int userId, string propertyId, decimal amount)
    {
        var client = new Razorpay.Api.RazorpayClient("rzp_test_enJO1baCLKAEqV", "gJkC53PjlaweN1tLBKKzyRDR");

        var options = new Dictionary<string, object>
        {
            { "amount", amount * 100 }, // in paise
            { "currency", "INR" },
            { "receipt", "txn_12345" },
            { "payment_capture", 1 }
        };

        try
        {
            var order = client.Order.Create(options);
            string orderId = order["id"].ToString();

            SavePayment(orderId, userId, propertyId, amount);

            ViewBag.orderId = orderId;
            ViewBag.Amount = amount;
            ViewBag.Key = "rzp_test_enJO1baCLKAEqV"; // for checkout script
            ViewBag.UserId = userId;

            return View("PaymentConfirmation");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.ToString()); // Debug logs
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    private void SavePayment(string orderId, int userId, string propertyId, decimal amount)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "INSERT INTO Payments (UserId, PropertyId, Amount, OrderId, PaymentStatus, PaymentDate) " +
                           "VALUES (@UserId, @PropertyId, @Amount, @OrderId, 'Pending', GETDATE())";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@PropertyId", propertyId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    [HttpPost]
    //public IActionResult VerifyPayment(string razorpayPaymentId, string OrderId, string razorpaySignature)
    //{
    //    try
    //    {
    //        bool isValid = VerifyPaymentSignature(OrderId, razorpayPaymentId, razorpaySignature);

    //        if (isValid)
    //        {
    //            UpdatePaymentStatus(razorpayPaymentId, OrderId);
    //            return View("PaymentSuccess");
    //        }
    //        else
    //        {
    //            return View("PaymentFailure");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ViewBag.ErrorMessage = ex.Message;
    //        return View("Error");
    //    }
    //}

    public IActionResult VerifyPayment(string razorpayPaymentId, string orderId, string razorpaySignature)
    {
        try
        {
            // Directly update payment status without signature validation
            UpdatePaymentStatus(razorpayPaymentId, orderId);

            // Redirect to Index after success
            return RedirectToAction("Index", "Home"); // Replace "Home" with your actual controller if needed
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Payment update failed: " + ex.Message;
            return View("Error");
        }
    }



    //private bool VerifyPaymentSignature(string orderId, string paymentId, string signature)
    //{
    //    string secret = "gJkC53PjlaweN1tLBKKzyRDR";
    //    string payload = orderId + "|" + paymentId;

    //    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
    //    {
    //        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    //        string generatedSignature = Convert.ToBase64String(hash); // ✅ Base64 string required

    //        return generatedSignature == signature;
    //    }
    //}

    //private bool VerifyPaymentSignature(string orderId, string paymentId, string signature)
    //{
    //    string secret = "gJkC53PjlaweN1tLBKKzyRDR";
    //    string payload = orderId + "|" + paymentId;

    //    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
    //    {
    //        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    //        string generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
    //        return generatedSignature == signature;
    //    }
    //}

    private bool VerifyPaymentSignature(string orderId, string paymentId, string razorpaySignature)
    {
        try
        {
            string keySecret = _configuration["Razorpay:KeySecret"];
            string generatedSignature = GenerateSignature(orderId + "|" + paymentId, keySecret);

            return generatedSignature == razorpaySignature;
        }
        catch (Exception ex)
        {
            // Log or return message for debug
            throw new Exception("Signature verification failed: " + ex.Message);
        }
    }
    private string GenerateSignature(string data, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        using (var hmac = new HMACSHA256(keyBytes))
        {
            byte[] hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }



    private void UpdatePaymentStatus(string razorpayPaymentId, string orderId)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string query = "UPDATE Payments SET PaymentStatus = 'Success', RazorpayPaymentId = @PaymentId, PaymentDate = GETDATE() WHERE OrderId = @OrderId";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@PaymentId", razorpayPaymentId);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.ExecuteNonQuery();
            }
        }
    }

}