using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (model.Phone == "9080605040" && model.Password == "111111")
            {
                return RedirectToAction("Dashboard", "Account");

            }

            else if (model.Phone == "1234567890" && model.Password == "123456")
            {
                return RedirectToAction("Super_AdminDashboard", "Account");
            }

            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Super_AdminDashboard()
        {
            return View();
        }

    }
}