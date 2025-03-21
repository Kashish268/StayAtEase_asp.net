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
        public IActionResult Reviews()
        { 
        
                 return View();
        }
        public IActionResult Property_List()
        {
            return View(); 
        }
        public IActionResult Messages() {
            return View();
        }
    }
}
