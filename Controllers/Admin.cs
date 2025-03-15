using Microsoft.AspNetCore.Mvc;
using static WebApplication1.Models.DashboardModel;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Admin : Controller
    {
        public IActionResult Index()
        {
            var model = new AdminDashboardModel
            {
                TotalProperties = 24,
                ActiveListings = 18,
                TotalInquiries = 156,
                ReviewRating = 4.8,
                Messages = new List<Message>
                {
                    new Message { Name = "Sarah Johnson", PropertyId = "P.101", Email = "sarahjohnson@gmail.com", Contact = "1234567890", MessageText = "Are there any restrictions in lease agreements?" },
                    new Message { Name = "Michael Brown", PropertyId = "P.102", Email = "michaelbrown@gmail.com", Contact = "9876543210", MessageText = "Are there any restrictions on lease agreements?" },
                    new Message { Name = "Emma Davis", PropertyId = "P.103", Email = "emmadavis@gmail.com", Contact = "7410258963", MessageText = "What documents are required for booking?" }
                },
                Reviews = new List<Review>
                {
                    new Review { Name = "John Smith", Date = "Feb 12, 2024", Rating = 5, Text = "Amazing property with stunning views. Highly recommended!", Property = "Lakefront Cottage", Img = "/images/profile.png" },
                    new Review { Name = "Lisa Anderson", Date = "Feb 8, 2024", Rating = 5, Text = "Great location and comfortable stay. Would visit again.", Property = "Downtown Loft", Img = "/images/profile.png" },
                    new Review { Name = "David Wilson", Date = "Feb 7, 2024", Rating = 5, Text = "Perfect getaway spot. Everything was exactly as described.", Property = "Mountain View Cabin", Img = "/images/profile.png" }
                }
            };

            return View(model);
        }
    }
}
