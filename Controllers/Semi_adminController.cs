using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Semi_adminController : Controller
    {
        public IActionResult Dashboard()
        {
            var model = new DashboardViewModel
            {
                TotalProperties = 24,
                ActiveListings = 18,
                TotalInquiries = 156,
                ReviewRating = 4.8,
                Messages = new List<PropertyMessageViewModel>
                {
                    new PropertyMessageViewModel { Name = "Sarah Johnson", PropertyId = "P_101", Email = "sarahjohnson@gmail.com", Contact = "1236547890", Message = "Are there any restrictions on lease agreements?" },
                    new PropertyMessageViewModel { Name = "Michael Brown", PropertyId = "P_102", Email = "michaelbrown@gmail.com", Contact = "9874563210", Message = "Are there any restrictions on lease agreements?" },
                    new PropertyMessageViewModel { Name = "Emma Davis", PropertyId = "P_103", Email = "emmadavis@gmail.com", Contact = "7410258963", Message = "What documents are required for booking?" }
                },
                Reviews = new List<PropertyReview>
                {
                    new PropertyReview { Name = "John Smith", Date = "Feb 10, 2024", Rating = 5, Text = "Amazing property with stunning views. Highly recommended!", Property = "Lakefront Cottage", ImageUrl = "/images/user1.jpg" },
                    new PropertyReview { Name = "Lisa Anderson", Date = "Feb 8, 2024", Rating = 4, Text = "Great location and comfortable stay. Would visit again.", Property = "Downtown Loft", ImageUrl = "/images/user2.jpg" },
                    new PropertyReview { Name = "David Wilson", Date = "Feb 7, 2024", Rating = 5, Text = "Perfect getaway spot. Everything was exactly as described.", Property = "Mountain View Cabin", ImageUrl = "/images/user3.jpg" }
                }
            };
            return View("Dashboard", model);
        }

        public IActionResult Add_Properties() { 
            return View();
        }
        public IActionResult Reviews()
        { 
        
                 return View();
        }
        public IActionResult Property_List()
        {
            var model = new MyListingsViewModel
            {
                TotalProperties = 10,
                ActiveListings = 7,
                TotalInquiries = 25,
                SearchTerm = "",
                Properties = new List<PropertyViewModel>
        {
            new PropertyViewModel
            {
                Id = 1,
                Title = "Luxury Apartment",
                Price = "25000",
                Area = 1200,
                Address = "Downtown, City",
                IsAvailable = true,
                ImageUrl = "/assets/Property1.jpg"
            },
            new PropertyViewModel
            {
                Id = 2,
                Title = "Modern Villa",
                Price = "50000",
                Area = 2000,
                Address = "Uptown, City",
                IsAvailable = false,
                ImageUrl = "/assets/Property2.jpg"
            },
            new PropertyViewModel
            {
                Id = 1,
                Title = "Luxury Apartment",
                Price = "25000",
                Area = 1200,
                Address = "Downtown, City",
                IsAvailable = true,
                ImageUrl = "/assets/Property1.jpg"
            },
            new PropertyViewModel
            {
                Id = 2,
                Title = "Modern Villa",
                Price = "50000",
                Area = 2000,
                Address = "Uptown, City",
                IsAvailable = false,
                ImageUrl = "/assets/Property2.jpg"

            },
            new PropertyViewModel
            {
                Id = 1,
                Title = "Luxury Apartment",
                Price = "25000",
                Area = 1200,
                Address = "Downtown, City",
                IsAvailable = true,
                ImageUrl = "/assets/Property1.jpg"
            },
            new PropertyViewModel
            {
                Id = 2,
                Title = "Modern Villa",
                Price = "50000",
                Area = 2000,
                Address = "Uptown, City",
                IsAvailable = false,
                ImageUrl = "/assets/Property2.jpg"
            },
            new PropertyViewModel
            {
                Id = 1,
                Title = "Luxury Apartment",
                Price = "25000",
                Area = 1200,
                Address = "Downtown, City",
                IsAvailable = true,
                ImageUrl = "/assets/Property1.jpg"
            },
            new PropertyViewModel
            {
                Id = 2,
                Title = "Modern Villa",
                Price = "50000",
                Area = 2000,
                Address = "Uptown, City",
                IsAvailable = false,
                ImageUrl = "/assets/Property2.jpg"
            },
            new PropertyViewModel
            {
                Id = 1,
                Title = "Luxury Apartment",
                Price = "25000",
                Area = 1200,
                Address = "Downtown, City",
                IsAvailable = true,
                ImageUrl = "/assets/Property1.jpg"
            },
            new PropertyViewModel
            {
                Id = 2,
                Title = "Modern Villa",
                Price = "50000",
                Area = 2000,
                Address = "Uptown, City",
                IsAvailable = false,
                ImageUrl = "/assets/Property2.jpg"
            },
        }
            };
            return View(model); 
        }
        public IActionResult Messages() {
            return View();
        }

        public IActionResult MyProfile()
        {
            var model = new ProfileDetailsViewModel
            {
                ProfileImageUrl = "/profile.png",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1 (555) 000-0000",
                Location = "San Francisco, CA",
                Timezone = "(GMT-08:00) Pacific Time",
                Bio = "Property dealer"
            };
            return View(model);
        }
    }
}
