using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

            //if (model.email == "admin@gmail.com" && model.password == "admin@123")
            //{
            //    return RedirectToAction("Super_AdminDashboard", "Account");
            //}

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            // string errorHtml;
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
        public async Task<IActionResult> Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }

            DashboardViewModel viewModel = new DashboardViewModel
            {
                Messages = new List<DashboardMessage>(),
                Reviews = new List<LatestReview>()
            };

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

                // Total Properties
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Properties WHERE UserId = @UserID", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    viewModel.TotalProperties = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                // Active Listings
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Properties WHERE UserId = @UserID AND IsAvailable = 0", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    viewModel.ActiveListings = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                // Total Inquiries
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Inquiries WHERE PropertyID IN (SELECT PropertyID FROM Properties WHERE UserId = @UserID)", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    viewModel.TotalInquiries = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                // Average Rating
                using (SqlCommand cmd = new SqlCommand("SELECT AVG(Rating) FROM Reviews WHERE PropertyID IN (SELECT PropertyID FROM Properties WHERE UserId = @UserID)", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    object avgResult = await cmd.ExecuteScalarAsync();
                    viewModel.AverageRating = avgResult != DBNull.Value ? Convert.ToDecimal(avgResult) : 0;
                }

                // Latest Messages
                string msgQuery = @"
            SELECT TOP 10 i.Message, i.PropertyID, u.name, u.email, u.mobile
            FROM Inquiries i
            JOIN Users u ON i.UserId = u.user_id
            WHERE i.PropertyID IN (SELECT PropertyID FROM Properties WHERE UserId = @UserID)
            ORDER BY i.InquiryDate DESC";

                using (SqlCommand cmd = new SqlCommand(msgQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            viewModel.Messages.Add(new DashboardMessage
                            {
                                Name = reader["name"].ToString(),
                                PropertyId = reader["PropertyID"].ToString(),
                                Email = reader["email"].ToString(),
                                Contact = reader["mobile"].ToString(),
                                MessageText = reader["Message"].ToString()
                            });
                        }
                    }
                }

                // Latest Reviews (Top 3)
                string reviewQuery = @"
            SELECT TOP 3 r.ReviewDate, r.Rating, r.Comment, u.name, u.profile_pic, p.Title
            FROM Reviews r
            JOIN Users u ON r.UserID = u.user_id
            JOIN Properties p ON r.PropertyID = p.PropertyID
            WHERE p.UserId = @UserID
            ORDER BY r.ReviewDate DESC";

                using (SqlCommand cmd = new SqlCommand(reviewQuery, con))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            viewModel.Reviews.Add(new LatestReview
                            {
                                Name = reader["name"].ToString(),
                                ProfileImage = reader["profile_pic"] != DBNull.Value ? reader["profile_pic"].ToString() : "/assets/profile.png",
                                Date = Convert.ToDateTime(reader["ReviewDate"]).ToString("MMM d, yyyy"),
                                Rating = Convert.ToDecimal(reader["Rating"]),
                                Comment = reader["Comment"].ToString(),
                                PropertyTitle = reader["Title"].ToString()
                            });
                        }
                    }
                }
            }

            return View("Dashboard", viewModel);
        }


        public IActionResult Super_AdminDashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new SuperAdminDashboardViewModel
            {
                Tenants = new List<User>(),
                PropertyOwners = new List<User>(),
                LatestMessages = new List<PropertyMessageViewModel>(),
                LatestReviews = new List<PropertyReview>()
            };

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Properties", conn))
                {
                    model.TotalProperties = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Properties WHERE IsAvailable = 1", conn))
                {
                    model.ActiveListings = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Inquiries", conn))
                {
                    model.TotalInquiries = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand("SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews", conn))
                {
                    var result = cmd.ExecuteScalar();
                    model.AverageReviewRating = result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }

                // Tenants
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 user_id, name, mobile, email, role, status, created_at, profile_pic FROM users WHERE role = 'tenant'", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Tenants.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Mobile = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4),
                            Status = reader.GetString(5),
                            CreatedAt = reader.GetDateTime(6),
                            ProfilePic = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }

                // Property Owners
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 user_id, name, mobile, email, role, status, created_at, profile_pic FROM users WHERE role = 'roomowner'", conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.PropertyOwners.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Mobile = reader.GetString(2),
                            Email = reader.GetString(3),
                            Role = reader.GetString(4),
                            Status = reader.GetString(5),
                            CreatedAt = reader.GetDateTime(6),
                            ProfilePic = reader.IsDBNull(7) ? null : reader.GetString(7)
                        });
                    }
                }

                // Latest Messages
                string messageQuery = @"
                    SELECT TOP 3 u.name, u.email, u.mobile, p.PropertyId, i.Message
                    FROM Inquiries i
                    JOIN users u ON i.UserID = u.user_id
                    JOIN Properties p ON i.PropertyID = p.PropertyId
                    ORDER BY i.InquiryDate DESC";

                using (SqlCommand cmd = new SqlCommand(messageQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.LatestMessages.Add(new PropertyMessageViewModel
                        {
                            Name = reader.GetString(0),
                            Email = reader.GetString(1),
                            Contact = reader.GetString(2),
                            PropertyId = "P-" + reader.GetInt32(3),
                            Message = reader.GetString(4)
                        });
                    }
                }

                // Latest Reviews
                string reviewQuery = @"
                    SELECT TOP 3 r.ReviewDate, r.Rating, r.Comment, u.name, u.Profile_Pic, p.Title
                    FROM Reviews r
                    JOIN Users u ON r.UserID = u.User_ID
                    JOIN Properties p ON r.PropertyID = p.PropertyID
                    ORDER BY r.ReviewDate DESC";

                using (SqlCommand cmd = new SqlCommand(reviewQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.LatestReviews.Add(new PropertyReview
                        {
                            Name = reader.GetString(3),
                            ImageUrl = reader.IsDBNull(4) ? "/assets/profile.png" : reader.GetString(4),
                            Date = Convert.ToDateTime(reader["ReviewDate"]).ToString("MMM d, yyyy"),
                            Rating = Convert.ToDecimal(reader["Rating"]),
                            ReviewText = reader.GetString(2),
                            PropertyTitle = reader.GetString(5)
                        });
                    }
                }
            }

            return View(model);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                ViewBag.ProfileImage = GetProfileImagePath(userId.Value);
            }
        }

        protected string GetProfileImagePath(int userId)
        {
            string profilePath = null;
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

            return profilePath ?? "/assets/profile.png";
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["LogoutMessage"] = "You have been logged out successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
