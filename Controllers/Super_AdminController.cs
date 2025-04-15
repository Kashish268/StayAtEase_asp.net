
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Super_AdminController : BaseController
    {
        private readonly IConfiguration _configuration;

        public Super_AdminController(IConfiguration configuration) : base(configuration)
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
        public IActionResult Super_AdminDashboard()
        {

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            
            ViewData["ActivePage"] = "Super_AdminDashboard";


            return View();
        }

        public IActionResult Total_User()
        {
            ViewData["ActivePage"] = "Total_User";

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

           
            return View();
        }
        public IActionResult Total_Admin()
        {
            ViewData["ActivePage"] = "Total_Admin";
            return View();
        }
        public IActionResult Property_Reviews()
        {

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

           
            ViewData["ActivePage"] = "Property_Reviews";
            return View();
        }
        public IActionResult Inquiries()
        {
            ViewData["ActivePage"] = "Inquiries";
            return View();
        }
        public IActionResult Total_Properties()
        {


            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

           
            ViewData["ActivePage"] = "Total_Properties";
            return View();
        }

        public IActionResult Particular_property()
        {
            ViewData["ActivePage"] = "Total_Properties";
            return View();
        }
        public IActionResult AdminProfile() {

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
    }
}
