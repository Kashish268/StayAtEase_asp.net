using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Configuration;

namespace WebApplication1.Controllers
{
    public class Semi_adminController : BaseController
    {
        private readonly IConfiguration _configuration;

        public Semi_adminController(IConfiguration configuration) : base(configuration)
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
        public IActionResult Dashboard()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

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

        public IActionResult Add_Properties()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }

        public IActionResult Reviews()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }

        public IActionResult Property_List(int currentPage = 1, int pageSize = 8)
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            var properties = new List<PropertyViewModel>
            {
                new PropertyViewModel { Id = 1, Title = "Luxury Apartment", Price = "25000", Area = 1200, Address = "Downtown, City", IsAvailable = true, ImageUrl = "/assets/Property1.jpg" },
                new PropertyViewModel { Id = 2, Title = "Modern Villa", Price = "50000", Area = 2000, Address = "Uptown, City", IsAvailable = false, ImageUrl = "/assets/Property2.jpg" },
                new PropertyViewModel { Id = 3, Title = "Luxury Apartment", Price = "25000", Area = 1200, Address = "Downtown, City", IsAvailable = true, ImageUrl = "/assets/Property1.jpg" },
                new PropertyViewModel { Id = 4, Title = "Modern Villa", Price = "50000", Area = 2000, Address = "Uptown, City", IsAvailable = false, ImageUrl = "/assets/Property2.jpg" },
            };

            var totalProperties = properties.Count;

            var model = new MyListingsViewModel
            {
                TotalProperties = totalProperties,
                ActiveListings = properties.Count(p => p.IsAvailable),
                TotalInquiries = 25,
                CurrentPage = currentPage,
                PageSize = pageSize,
                Properties = properties.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
            };

            return View(model);
        }

        public IActionResult Messages()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            ViewData["ActivePage"] = "Messages";
            return View();
        }

        public IActionResult MyProfile()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

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

        public IActionResult Property_Details()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }

        public IActionResult Edit_Property()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }
    
    }
}
