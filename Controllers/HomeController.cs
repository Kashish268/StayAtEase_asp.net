using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Home";
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["ActivePage"] = "Privacy";
            return View();
        }

        public IActionResult WishList()
        {
            ViewData["ActivePage"] = "WishList";
            return View();
        }

        public IActionResult Property_details()
        {
            ViewData["ActivePage"] = "Privacy";
            return View();
        }

        //public IActionResult Dashboard() { 
        //    return View();
        //}

        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Login(LoginModel model)
        //{
        //    if (model.Email == "admin@gmail.com" && model.Password == "111")
        //    {
        //        Console.WriteLine("Login Successfull");
        //        return RedirectToAction("Dashboard", "Semi_admin");
        //    }

        //    ViewBag.Error = "Invalid";
        //    return View();
        //}
        public IActionResult Register()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //    [HttpPost]
        //    public IActionResult Register(RegisterViewModel model) {
        //        if (!ModelState.IsValid)
        //        {
        //            return PartialView("Register", model);
        //        }

        //               return Json(new { success = true, message = "Registration successful!" });

        //    }

        //    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //    public IActionResult Error()
        //    {
        //        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //    }
    }
}
