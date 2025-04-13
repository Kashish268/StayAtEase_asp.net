using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        // ✅ Constructor correctly defined
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            // Check if model is valid
            if (string.IsNullOrWhiteSpace(model.email) || string.IsNullOrWhiteSpace(model.password))
            {
                //ViewBag.ErrorMessage = "Email and Password are required.";
                return View();
            }

            if (model.email == "admin@gmail.com" && model.password == "admin@123")
            {
                return RedirectToAction("Super_AdminDashboard", "Account");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM users WHERE email = @Email AND password = @Password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", model.email);
                    cmd.Parameters.AddWithValue("@Password", model.password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        string status = reader["status"].ToString();
                        string userType = reader["role"].ToString();

                        if (status != "Active")
                        {
                            ViewBag.ErrorMessage = "Kindly activate your account by checking your email.";
                            return View();
                        }

                        switch (userType.ToLower())
                        {
                            case "roomowner":
                                return RedirectToAction("Dashboard", "Account");

                            case "tenant":
                                return RedirectToAction("Index", "Home");

                            default:
                                ViewBag.ErrorMessage = "Unauthorized user type.";
                                return View();
                        }
                    }
                }
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
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
