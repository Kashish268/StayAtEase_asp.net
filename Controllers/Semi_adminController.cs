using Microsoft.AspNetCore.Mvc;

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
    }
}
