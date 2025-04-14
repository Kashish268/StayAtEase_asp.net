
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Super_AdminController : BaseController
    {
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
