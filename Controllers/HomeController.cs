using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
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
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            string errorHtml;
            if (!ModelState.IsValid)
            {
               
                if (model.password != model.confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Password and Confirm Password do not match.");
                    errorHtml = "<div class='alert alert-danger'>Password and Confirm Password do not match.<br/></div>";



                }
                else {
                    errorHtml = "<div class='alert alert-danger'>Please enter all required fields correctly.</div>";

                }
                // You can loop through ModelState if you want detailed errors, but for simplicity:
                return Content(errorHtml);
            }


            string token = Guid.NewGuid().ToString();
            string verificationLink = Url.Action("VerifyEmail", "Home", new { token }, Request.Scheme);
            string emailBody = $@"
    <p>Welcome to <strong>StayAtEase {model.fullname}</strong>!</p>
    <p>You are registered as a <strong>{model.userType}</strong>.</p>
    <p>Kindly click the link below to activate your account:</p>
    <p><a href='{verificationLink}'>Activate My Account</a></p>
    <p><em>Note: This link will expire in 24 hours.</em></p>
    <br/>
    <p>Thank you,<br/>StayAtEase Team</p>";

            bool emailSent = SendVerificationEmail(model.email, "Email Verification", emailBody);

            if (!emailSent)
            {
                errorHtml = "Failed to send verification email. Please try again later.";
                //return PartialView("_RegisterPartial", model);
                return Content(errorHtml);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if email already exists
                string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                using (SqlCommand cmd = new SqlCommand(checkEmailQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", model.email);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        errorHtml = "<div class='alert alert-danger'>This email is already registered. Please use a different email.</div>";
                        return Content(errorHtml);
                        //return PartialView("_RegisterPartial", model);
                    }
                }

                // Check if phone already exists
                string checkPhoneQuery = "SELECT COUNT(*) FROM users WHERE mobile = @Phone";
                using (SqlCommand phoneCmd = new SqlCommand(checkPhoneQuery, conn))
                {
                    phoneCmd.Parameters.AddWithValue("@Phone", model.phone);
                    int phoneCount = (int)phoneCmd.ExecuteScalar();

                    if (phoneCount > 0)
                    {
                        //string phoneError = "<div class='alert alert-danger'>This contact number is already registered. Please use a different one.</div>";
                        //return Content(phoneError);


                        ViewBag.ValidationErrors = "This phone number is already in use.";
                        return PartialView("_RegisterPartial", model); // ✅ brings back form + error
                    }
                }


                // Insert only after email is sent successfully
                string insertQuery = @"INSERT INTO users (name, email, mobile, password, role, verification_token, token_expiration)
                               VALUES (@Name, @Email, @Phone, @Password, @Role, @Token,DATEADD(DAY, 1, GETDATE())
)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", model.fullname);
                    cmd.Parameters.AddWithValue("@Email", model.email);
                    cmd.Parameters.AddWithValue("@Phone", model.phone);
                    cmd.Parameters.AddWithValue("@Password", model.password); // Should hash in real apps
                    cmd.Parameters.AddWithValue("@Role", model.userType);
                    cmd.Parameters.AddWithValue("@Token", token);

                    cmd.ExecuteNonQuery();
                }
            }

            return Content("<div class='alert alert-success'>Registration successful! Please check your email to activate your account.</div>");
        }


        private bool SendVerificationEmail(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("flatmate110@gmail.com", "pfhy afpq bnam rfde"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("flatmate110@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Optionally log
                return false;
            }
        }

        public IActionResult VerifyEmail(string token)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if the token is valid and not expired
                string checkTokenQuery = "SELECT COUNT(*) FROM users WHERE verification_token = @Token AND token_expiration > GETDATE()";
                using (SqlCommand cmd = new SqlCommand(checkTokenQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    int count = (int)cmd.ExecuteScalar();

                    if (count == 0)
                    {
                        // Pass flag to view to indicate expired/invalid
                        ViewBag.IsTokenValid = false;
                        ViewBag.ErrorMessage = "This verification link has expired or is invalid. Please register again.";
                        return View("EmailVerified");
                    }
                }

                // Activate the user account after successful token verification
                string updateQuery = "UPDATE users SET status = 'Active' WHERE verification_token = @Token";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.IsTokenValid = true;
                ViewBag.SuccessMessage = "Your account is now active!";
            }

            return View("EmailVerified");
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
