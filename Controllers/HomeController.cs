using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;


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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                ViewBag.ProfileImage = GetProfileImagePath(userId.Value);
            }

            base.OnActionExecuting(context);
        }

        protected string GetProfileImagePath(int userId)
        {
            string profilePath = null;

            // Shared connection string (update with your actual connection string)
            string connectionString = _configuration.GetConnectionString("DefaultConnection");


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT profile_pic FROM users WHERE user_id = @UserId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    profilePath = result.ToString();
                }
            }

            return profilePath;
        }

        public IActionResult Index()
        {
            ViewData["ActivePage"] = "Home";

            List<UserProperty> featuredProperties = new List<UserProperty>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT TOP 6 
                p.PropertyId, 
                p.Title, 
                p.Address AS Location, 
                p.Price,
                p.ImagePaths AS Image,
                CAST(ROUND(ISNULL(AVG(r.Rating), 0), 1) AS DECIMAL(2,1)) AS Rating,
                p.PropertyType
            FROM Properties p
            LEFT JOIN Reviews r ON p.PropertyId = r.PropertyID
            GROUP BY 
                p.PropertyId, p.Title, p.Address, p.Price, p.ImagePaths, p.PropertyType
            ORDER BY NEWID()";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // Safe conversions with null checks
                    decimal price = 0.0m;
                    decimal.TryParse(reader["Price"]?.ToString(), out price);

                    decimal rating = 0.0m;
                    decimal.TryParse(reader["Rating"]?.ToString(), out rating);

                    featuredProperties.Add(new UserProperty
                    {
                        PropertyID = reader["PropertyId"] != DBNull.Value ? Convert.ToInt32(reader["PropertyId"]) : 0,
                        Title = reader["Title"]?.ToString() ?? "Untitled Property",
                        Location = reader["Location"]?.ToString() ?? "Location not specified",
                        Price = "₹" + price.ToString("N0") + "/month",  // Changed $ to ₹
                        ImagePath = reader["Image"]?.ToString() ?? "/assets/default-property.jpg",
                        Rating = rating,
                        PropertyType = reader["PropertyType"]?.ToString() ?? "Unknown"
                    });
                }
            }

            return View(featuredProperties);
        }

        //private string GetProfileImagePath(int? userId)
        //{
        //    if (userId == null) return null;

        //    string profilePath = null;
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT profile_pic FROM users WHERE user_id = @UserId";

        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@UserId", userId.Value);

        //        conn.Open();
        //        object result = cmd.ExecuteScalar();
        //        if (result != null && result != DBNull.Value)
        //        {
        //            profilePath = result.ToString();
        //        }
        //    }

        //    return profilePath;
        //}


        public IActionResult Privacy()
        {
            List<UserProperty> properties = new List<UserProperty>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int? userId = HttpContext.Session.GetInt32("UserId"); // Login check

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query;

                if (userId != null)
                {
                    // If user is logged in, join Wishlist and get IsWishlisted
                    query = @"
            SELECT 
                p.PropertyId, 
                p.Title, 
                p.Address AS Location, 
                p.Bedrooms AS Beds, 
                p.Bathrooms AS Baths, 
                CAST(p.SquareFootage AS NVARCHAR) + ' sqft' AS Size, 
                N'₹' + CAST(p.Price AS NVARCHAR) + '/month' AS Price, 
                p.ImagePaths AS ImagePath, 
                CAST(ROUND(ISNULL(AVG(r.Rating), 0), 1) AS DECIMAL(2,1)) AS Rating, 
                COUNT(r.ReviewID) AS Reviews,
                CASE WHEN w.UserID IS NOT NULL THEN 1 ELSE 0 END AS IsWishlisted
            FROM Properties p
            LEFT JOIN Reviews r ON p.PropertyId = r.PropertyID
            LEFT JOIN Wishlist w ON p.PropertyId = w.PropertyID AND w.UserId = @UserId
            GROUP BY 
                p.PropertyId, p.Title, p.Address, p.Bedrooms, p.Bathrooms, 
                p.SquareFootage, p.Price, p.ImagePaths, w.UserId";
                }
                else
                {
                    // If user is NOT logged in, IsWishlisted will always be false
                    query = @"
            SELECT 
                p.PropertyId, 
                p.Title, 
                p.Address AS Location, 
                p.Bedrooms AS Beds, 
                p.Bathrooms AS Baths, 
p.PropertyType ,
                CAST(p.SquareFootage AS NVARCHAR) + ' sqft' AS Size, 
                N'₹' + CAST(p.Price AS NVARCHAR) + '/month' AS Price, 
                p.ImagePaths AS ImagePath, 
                CAST(ROUND(ISNULL(AVG(r.Rating), 0), 1) AS DECIMAL(2,1)) AS Rating, 
                COUNT(r.ReviewID) AS Reviews,
                0 AS IsWishlisted
            FROM Properties p
            LEFT JOIN Reviews r ON p.PropertyId = r.PropertyID
            GROUP BY 
                p.PropertyId, p.Title, p.Address, p.Bedrooms, p.Bathrooms, 
                p.SquareFootage, p.Price, p.ImagePaths, p.PropertyType";
                }

                SqlCommand cmd = new SqlCommand(query, conn);

                if (userId != null)
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                }

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    properties.Add(new UserProperty
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString(),
                        Location = reader["Location"].ToString(),
                        Beds = Convert.ToInt32(reader["Beds"]),
                        Baths = Convert.ToInt32(reader["Baths"]),
                        Size = reader["Size"].ToString(),
                        Price = reader["Price"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                        PropertyType = reader["PropertyType"].ToString(),
                        Rating = Convert.ToDecimal(reader["Rating"]),
                        Reviews = Convert.ToInt32(reader["Reviews"]),
                        IsWishlisted = Convert.ToInt32(reader["IsWishlisted"]) == 1
                    });
                }
            }

            ViewData["ActivePage"] = "Privacy";
            return View(properties);
        }

        public IActionResult WishList()
        {
            ViewData["ActivePage"] = "WishList";

            var userId = HttpContext.Session.GetInt32("UserId");
            bool isLoggedIn = userId.HasValue;
            ViewData["IsLoggedIn"] = isLoggedIn;

            // if user is not logged in, skip DB call and return empty list
            if (!isLoggedIn)
            {
                return View(new List<UserProperty>());
            }

            List<UserProperty> wishlistedProperties = new List<UserProperty>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
SELECT 
    p.PropertyId, 
    p.Title, 
    p.Address AS Location, 
    p.Bedrooms AS Beds, 
    p.Bathrooms AS Baths, 
    CAST(p.SquareFootage AS NVARCHAR) + ' sqft' AS Size, 
    N'₹' + CAST(p.Price AS NVARCHAR) + '/month' AS Price, 
    p.ImagePaths AS ImagePath, 
    CAST(ROUND(ISNULL(AVG(r.Rating), 0), 1) AS DECIMAL(2,1)) AS Rating, 
    COUNT(r.ReviewID) AS Reviews
FROM Properties p
INNER JOIN Wishlist w ON p.PropertyId = w.PropertyID
LEFT JOIN Reviews r ON p.PropertyId = r.PropertyID
WHERE w.UserId = @UserId
GROUP BY 
    p.PropertyId, p.Title, p.Address, p.Bedrooms, p.Bathrooms, 
    p.SquareFootage, p.Price, p.ImagePaths";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId.Value); // .Value is safe here

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    wishlistedProperties.Add(new UserProperty
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString(),
                        Location = reader["Location"].ToString(),
                        Beds = Convert.ToInt32(reader["Beds"]),
                        Baths = Convert.ToInt32(reader["Baths"]),
                        Size = reader["Size"].ToString(),
                        Price = reader["Price"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                        Rating = Convert.ToDecimal(reader["Rating"]),
                        Reviews = Convert.ToInt32(reader["Reviews"])
                    });
                }
            }

            return View(wishlistedProperties);
        }



        //public IActionResult Property_details()
        //{
        //    ViewData["ActivePage"] = "Privacy";
        //    return View();
        //}

        public IActionResult Property_details(int id)
        {
            UserProperty property = null;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            p.PropertyId, 
            p.Title, 
            p.Address AS Location, 
            p.Bedrooms AS Beds, 
            p.Bathrooms AS Baths, 
            CAST(p.SquareFootage AS NVARCHAR) + ' sqft' AS Size, 
            N'₹' + CAST(p.Price AS NVARCHAR) + '/month' AS Price, 
            p.ImagePaths AS ImagePath, 
            CAST(ROUND(ISNULL(AVG(r.Rating), 0), 1) AS DECIMAL(2,1)) AS Rating, 
            COUNT(r.ReviewID) AS Reviews
        FROM Properties p
        LEFT JOIN Reviews r ON p.PropertyId = r.PropertyID
        WHERE p.PropertyId = @PropertyId
        GROUP BY 
            p.PropertyId, p.Title, p.Address, p.Bedrooms, p.Bathrooms, 
            p.SquareFootage, p.Price, p.ImagePaths";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyId", id);  // Passing the PropertyId to the query

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    property = new UserProperty
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString(),
                        Location = reader["Location"].ToString(),
                        Beds = Convert.ToInt32(reader["Beds"]),
                        Baths = Convert.ToInt32(reader["Baths"]),
                        Size = reader["Size"].ToString(),
                        Price = reader["Price"].ToString(),
                        ImagePath = reader["ImagePath"].ToString(),
                        Rating = Convert.ToDecimal(reader["Rating"]),
                        Reviews = Convert.ToInt32(reader["Reviews"])
                    };
                }
            }

            if (property == null)
            {
                return NotFound();  // If the property was not found, return a 404 error.
            }

            return View(property);  // Passing the property details to the view.
        }


        [HttpPost]
        public IActionResult SubmitReview(int propertyId, int userId, int rating, string comment)
        {
            if (rating == 0 || string.IsNullOrWhiteSpace(comment))
            {
                TempData["ErrorMessage"] = "Rating and comment are required.";
                return RedirectToAction("Property_details", new { id = propertyId });
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = @"INSERT INTO Reviews (PropertyID, UserID, Rating, Comment, ReviewDate)
                             VALUES (@PropertyID, @UserID, @Rating, @Comment, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PropertyID", propertyId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Rating", rating);
                        cmd.Parameters.AddWithValue("@Comment", comment);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Review submitted successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Error submitting review: " + ex.Message;
            }

            return RedirectToAction("Property_details", new { id = propertyId });
        }


        [HttpPost]
        public IActionResult SubmitInquiry(int userId, int propertyId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["InquiryError"] = "Please enter a message.";
                return RedirectToAction("Property_details", new { id = propertyId });
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = @"INSERT INTO Inquiries (UserID, PropertyID, Message, InquiryDate)
                             VALUES (@UserID, @PropertyID, @Message, @InquiryDate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@PropertyID", propertyId);
                        cmd.Parameters.AddWithValue("@Message", message);
                        cmd.Parameters.AddWithValue("@InquiryDate", DateTime.Now);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["InquirySuccess"] = "Inquiry sent successfully!";
            }
            catch (Exception ex)
            {
                TempData["InquiryError"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Property_details", new { id = propertyId });
        }


        [HttpGet]
        public IActionResult Profile_details()
        {
            ViewData["Title"] = "Profile_details";
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            UserProfile model = new UserProfile();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT name, email, mobile, profile_pic FROM users WHERE user_id = @UserId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.UserId = userId;
                    model.FirstName = reader["name"].ToString();
                    model.Email = reader["email"].ToString();
                    model.Phone = reader["mobile"].ToString();
                    model.ProfileImage = reader["profile_pic"].ToString();
                    //model.CreateAt = Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                con.Close();
            }

            return View(model);
        }

       
        [HttpPost]
        public IActionResult Profile_details(UserProfile model, IFormFile ProfilePicFile)
        {
            string formType = Request.Form["FormType"];

            if (formType == "ProfileUpdate")
            {
                // Manual validation for Profile Update
                if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.Phone))
                {
                    ViewBag.Error = "First name and phone number are required.";
                    return View(model);
                }

                string imagePathInDb = model.ProfileImage;

                if (ProfilePicFile != null && ProfilePicFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfilePicFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfilePicFile.CopyTo(stream);
                    }

                    imagePathInDb = "/images/" + uniqueFileName;
                }

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string updateQuery = "UPDATE users SET name = @FirstName, mobile = @Phone, profile_pic = @ProfilePic WHERE user_id = @UserId";
                    SqlCommand cmd = new SqlCommand(updateQuery, con);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@Phone", model.Phone);
                    cmd.Parameters.AddWithValue("@ProfilePic", imagePathInDb ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                model.ProfileImage = imagePathInDb;
                ViewBag.Message = "Profile updated successfully!";
                return View(model);
            }

            else if (formType == "PasswordChange")
            {
                if (string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    ViewBag.Error = "Please fill in all password fields.";
                    FillUserProfileData(model);
                    return View(model);
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ViewBag.Error = "New password and confirm password do not match.";
                    FillUserProfileData(model);
                    return View(model);
                }

                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string checkPasswordQuery = "SELECT password FROM users WHERE user_id = @UserId";
                    SqlCommand checkCmd = new SqlCommand(checkPasswordQuery, con);
                    checkCmd.Parameters.AddWithValue("@UserId", model.UserId);

                    con.Open();
                    var existingPassword = checkCmd.ExecuteScalar()?.ToString();
                    con.Close();

                    if (existingPassword != model.CurrentPassword)
                    {
                        ViewBag.Error = "Incorrect current password.";
                        FillUserProfileData(model);
                        return View(model);
                    }

                    string updatePasswordQuery = "UPDATE users SET password = @NewPassword WHERE user_id = @UserId";
                    SqlCommand updatePassCmd = new SqlCommand(updatePasswordQuery, con);
                    updatePassCmd.Parameters.AddWithValue("@NewPassword", model.NewPassword);
                    updatePassCmd.Parameters.AddWithValue("@UserId", model.UserId);

                    con.Open();
                    updatePassCmd.ExecuteNonQuery();
                    con.Close();
                }

                ViewBag.Message = "Password updated successfully!";
                FillUserProfileData(model);
                return View(model);
            }

            ViewBag.Error = "Unknown form submitted.";
            FillUserProfileData(model);
            return View("Index","Home");
        }


        private void FillUserProfileData(UserProfile model)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT name, email, mobile FROM users WHERE user_id = @UserId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", model.UserId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model.FirstName = reader["name"].ToString();
                    model.Email = reader["email"].ToString();
                    model.Phone = reader["mobile"].ToString();
                }
                con.Close();
            }
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
        public async Task<IActionResult> Register(RegisterViewModel model)
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
                    foreach (var state in ModelState.Values)
                    {
                        foreach (var error in state.Errors)
                        {
                            errorHtml += $"<li>{error.ErrorMessage}</li>";
                        }
                    }

                    errorHtml += "</ul></div>";

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
                // Insert only after email is sent successfully
                string insertQuery = @"INSERT INTO users (name, email, mobile, password, role, verification_token, token_expiration, profile_pic)
                               VALUES (@Name, @Email, @Phone, @Password, @Role, @Token, DATEADD(DAY, 1, GETDATE()), @ProfilePic)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", model.fullname);
                    cmd.Parameters.AddWithValue("@Email", model.email);
                    cmd.Parameters.AddWithValue("@Phone", model.phone);
                    cmd.Parameters.AddWithValue("@Password", model.password); // Hashing is recommended in production
                    cmd.Parameters.AddWithValue("@Role", model.userType);
                    cmd.Parameters.AddWithValue("@Token", token);

                    // ✅ Always save the default profile picture
                    string profilePicPath = "/assets/profile_default.jpg";

                    // ❌ No need to check or upload anything
                    cmd.Parameters.AddWithValue("@ProfilePic", profilePicPath);

                    //string profilePicPath;

                    //if (model.profilePicture != null && model.profilePicture.Length > 0)
                    //{
                    //    var ext = Path.GetExtension(model.profilePicture.FileName);
                    //    string uniqueFileName = Guid.NewGuid().ToString() + ext;

                    //    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", uniqueFileName);
                    //    Console.WriteLine("Saving file to: " + savePath);

                    //    using (var stream = new FileStream(savePath, FileMode.Create))
                    //    {
                    //        await model.profilePicture.CopyToAsync(stream);
                    //    }

                    //    profilePicPath = "/uploads/" + uniqueFileName;
                    //}
                    //else
                    //{
                    //    profilePicPath = "/assets/profile_default.png"; // Use default image only if image not uploaded
                    //}


                    //cmd.Parameters.AddWithValue("@ProfilePic", profilePicPath);

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

        [HttpGet]
        public IActionResult Forgotpassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string Email)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string userName = "";
                // Check if the email exists and fetch fullname
                string getUserQuery = "SELECT name FROM users WHERE email = @Email";
                using (SqlCommand cmd = new SqlCommand(getUserQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    var result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        ViewBag.Error = "Email is not valid.";
                        return View();
                    }

                    userName = result.ToString();
                }

                // Generate token and send email
                string token = Guid.NewGuid().ToString();
                string resetLink = Url.Action("Resetpassword", "Home", new { token = token }, Request.Scheme);

                string emailBody = $@"
        <p>Hello <strong>{userName}</strong>,</p>
        <p>We received a request to reset your password.</p>
        <p>Click the link below to reset it:</p>
        <p><a href='{resetLink}'>Reset Password</a></p>
        <p><em>Note: This link will expire in 1 hour for your security.</em></p>
        <br/>
        <p>If you didn’t request this, please ignore this email.</p>
        <p>Thanks,<br/>StayAtEase Team</p>";

                bool mailSent = SendVerificationEmail(Email, "Reset Password", emailBody);

                if (mailSent)
                {
                    // Save token in database (optional but recommended)
                    string updateTokenQuery = "UPDATE users SET verification_token = @Token, token_expiration = DATEADD(DAY, 1, GETDATE()) WHERE email = @Email";
                    using (SqlCommand updateCmd = new SqlCommand(updateTokenQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@Token", token);
                        updateCmd.Parameters.AddWithValue("@Email", Email);
                        updateCmd.ExecuteNonQuery();
                    }

                    ViewBag.Message = "Reset link has been sent to your email.";
                }
                else
                {
                    ViewBag.Error = "Failed to send email. Try again.";
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Resetpassword() {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, string Password, string ConfirmPassword)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            if (Password.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters long.";
                return View();
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check token validity
                string email = null;
                string tokenQuery = "SELECT email FROM users WHERE verification_token = @Token AND token_expiration > GETDATE()";
                using (SqlCommand cmd = new SqlCommand(tokenQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", token);
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        ViewBag.Error = "Your reset link has expired. Please request a new one.";
                        return RedirectToAction("Forgotpassword", "Home"); // adjust action/controller if needed
                    }

                    

                    email = result.ToString();
                }

                // Update password
                string updateQuery = "UPDATE users SET password = @Password WHERE email = @Email";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", Password); // Optional: hash password here
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.ExecuteNonQuery();
                }

                ViewBag.Message = "Your password has been successfully updated.";
            }

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
