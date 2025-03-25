using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Semi_adminController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewData["ActivePage"] = "Semi_AdminDashboard";
            return View();
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
                ImageUrl = "https://via.placeholder.com/50"
            },
            new PropertyViewModel
            {
                Id = 2,
                Title = "Modern Villa",
                Price = "50000",
                Area = 2000,
                Address = "Uptown, City",
                IsAvailable = false,
                ImageUrl = "https://via.placeholder.com/50"
            }
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
