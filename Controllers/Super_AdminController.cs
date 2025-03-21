using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class Super_AdminController : Controller
    {
        public IActionResult Super_AdminDashboard()
        {
            return View();
        }
    }
}
