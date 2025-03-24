
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Super_AdminController : Controller
    {
        public IActionResult Super_AdminDashboard()
        {
            ViewData["ActivePage"] = "Super_AdminDashboard";


            return View();
        }

        public IActionResult Total_User()
        {
            ViewData["ActivePage"] = "Total_User";
            return View();
        }
    }
}
