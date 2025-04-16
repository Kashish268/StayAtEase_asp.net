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
                //return Json(new { success = true, message = "Email and Password are required." });

                return View();
            }

<<<<<<< HEAD
            else if (model.Phone == "1234567890" && model.Password == "123456")
=======
            //if (model.email == "admin@gmail.com" && model.password == "admin@123")
            //{
            //    return RedirectToAction("Super_AdminDashboard", "Account");
            //}

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
           // string errorHtml;
            using (SqlConnection conn = new SqlConnection(connectionString))
>>>>>>> 8e8832b3f8929bee77889b849937ac7759ab7bbf
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

                        // ✅ Store user ID in session
                        int userId = Convert.ToInt32(reader["user_id"]);
                        HttpContext.Session.SetInt32("UserId", userId);

                        if (status != "Active")
                        {
                            //ViewBag.ErrorMessage = "Kindly activate your account by checking your email.";
                            //errorHtml = "<div class='alert alert-danger'>Kindly activate your account by checking your email.<br/></div>";
                            //return Content(errorHtml);

                            return Json(new { success = false, message = "Kindly activate your account by checking your email." });

                            //return View();
                        }

                        switch (userType.ToLower())
                        {
                            case "roomowner":
                                //return RedirectToAction("Dashboard", "Account");
                                return Json(new { success = true, redirectUrl = Url.Action("Dashboard", "Account") });


                            case "tenant":
                                //return RedirectToAction("Index", "Home");
                                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });


                            case "admin":
                                return Json(new { success = true, redirectUrl = Url.Action("Super_AdminDashboard", "Account") });

                            default:
                                return Json(new { success = false, message = "Unauthorized user type." });
                               
                        }
                    }
                }
            }

            //ViewBag.ErrorMessage = "Invalid email or password.";
            //errorHtml = "<div class='alert alert-danger'>Invalid email or password.<br/></div>";
            //return Content(errorHtml);
            //return View();
            return Json(new { success = false, message = "Invalid email or password." });

        }



        //public IActionResult Dashboard()
        //{
        //    return View();
        //}

        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // ✅ Session value print karne ke liye (console pe ya debug)
            Console.WriteLine("Logged in User ID: " + userId);

            // OR pass karo view me dekhne ke liye
            ViewBag.UserId = userId;

            return View();
        }


        public IActionResult Super_AdminDashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // ✅ Session value print karne ke liye (console pe ya debug)
            Console.WriteLine("Logged in User ID: " + userId);

            // OR pass karo view me dekhne ke liye
            ViewBag.UserId = userId;

            return View();
        }
    }
}
