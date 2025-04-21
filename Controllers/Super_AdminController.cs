using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Data.SqlClient;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Super_AdminController : BaseController
    {

        private readonly IConfiguration _config;
        private readonly ILogger<Super_AdminController> _logger;

       

        private readonly IConfiguration _configuration;

        public Super_AdminController(IConfiguration configuration) : base(configuration)
        {
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
        public IActionResult Super_AdminDashboard()
        {

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            
            ViewData["ActivePage"] = "Super_AdminDashboard";


            return View();
        }

        public IActionResult Total_User()
        {
            ViewData["ActivePage"] = "Total_User";

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;


            List< User> tenants = new List<User>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT user_id, name, mobile, created_at, status FROM users WHERE role = 'Tenant'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tenants.Add(new User
                        {
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Name = reader["name"].ToString(),
                            Mobile = reader["mobile"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            Status = reader["status"].ToString()
                        });
                    }
                }
            }

            return View(tenants);
        }
        [HttpPost]
        public IActionResult Total_User(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM users WHERE user_id = @UserId AND role = 'Tenant'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["Success"] = "User deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: " + id);
                TempData["Error"] = "Error deleting user.";
            }

            return RedirectToAction("Total_User");
        }
        [HttpGet]
        public IActionResult Total_Admin()
        {
            ViewData["ActivePage"] = "Total_Admin";

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            List<User> roomOwners = new List<User>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT user_id, name, mobile, created_at, status FROM users WHERE role = 'RoomOwner'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roomOwners.Add(new User
                        {
                            UserId = Convert.ToInt32(reader["user_id"]),
                            Name = reader["name"].ToString(),
                            Mobile = reader["mobile"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"]),
                            Status = reader["status"].ToString()
                        });
                    }
                }
            }

            return View(roomOwners);
        }

        [HttpPost]
        public IActionResult Total_Admin(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM users WHERE user_id = @UserId AND role = 'RoomOwner'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["Success"] = "Room owner deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting room owner with ID: {id}");
                TempData["Error"] = "Error deleting room owner.";
            }

            return RedirectToAction("Total_Admin");
        }
        [HttpGet]
        public async Task<IActionResult> Property_Reviews(string? searchTerm, int page = 1)
        {
            var reviews = new List<PropertyReview>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int pageSize = 10;
            int offset = (page - 1) * pageSize;

            int totalReviewsCount = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

                string query = @"
            SELECT r.ReviewID, u.Name, r.ReviewDate, r.Rating, r.Comment, p.PropertyID, p.Title AS PropertyTitle, u.profile_pic
            FROM Reviews r
            INNER JOIN Properties p ON p.PropertyID = r.PropertyID
            INNER JOIN Users u ON u.user_id = r.UserID
            ORDER BY r.ReviewDate DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reviews.Add(new PropertyReview
                            {
                                Id = Convert.ToInt32(reader["ReviewID"]),
                                Name = reader["Name"].ToString(),
                                Property = reader["PropertyID"].ToString(),
                                PropertyTitle = reader["PropertyTitle"].ToString(),  // Ensure PropertyTitle is assigned
                                Rating = Convert.ToInt32(reader["Rating"]),
                                ReviewText = reader["Comment"].ToString(),
                                ImageUrl = reader["profile_pic"] != DBNull.Value ? reader["profile_pic"].ToString() : string.Empty,
                                Date = reader["ReviewDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReviewDate"]).ToString("yyyy-MM-dd") : string.Empty,
                            });
                        }
                    }
                }
            }

            return View(reviews);
        }


        [HttpPost]
        public IActionResult Property_Reviews(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Reviews WHERE ReviewID = @ReviewID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReviewID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Property_Reviews");
        }

        public async Task<IActionResult> Inquiries(string? searchTerm, int page = 1, string filter = "all")
        {
            ViewData["ActivePage"] = "Inquiries";

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserId = userId;

            // Pagination settings
            int pageSize = 5; // Number of messages per page
            int skip = (page - 1) * pageSize;

            var messages = new List<PropertyMessageViewModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Base query for fetching messages
                string query = @"
            SELECT 
                m.Message,
                m.PropertyID,
                p.Title,
                p.ImagePaths,
                u.name,
                u.Email,
                u.mobile
            FROM Inquiries m
            INNER JOIN Properties p ON p.PropertyId = m.PropertyID
            INNER JOIN users u ON u.user_id = m.UserID
            ";

                // Search filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND m.Message LIKE @SearchTerm";
                }

                // Status filter (Read/Unread)
                if (filter == "unread")
                {
                    query += " AND m.Status = 'Unread'";
                }
                else if (filter == "read")
                {
                    query += " AND m.Status = 'Read'";
                }

                // Pagination logic
                query += " ORDER BY m.InquiryDate DESC OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Skip", skip);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            messages.Add(new PropertyMessageViewModel
                            {
                                Name = reader["name"].ToString(),                      // from users table
                                PropertyId = reader["PropertyID"].ToString(),          // from inquiries
                                Email = reader["email"].ToString(),                    // from users table
                                Contact = reader["mobile"].ToString(),                 // from users table
                                Message = reader["Message"].ToString(),                // from inquiries
                                PropertyTitle = reader["Title"].ToString(),            // from properties
                                ImageUrl = reader["ImagePaths"].ToString()
                                    ?.Split(',').FirstOrDefault()                    // first image from comma-separated list
                            });
                        }
                    }
                }

                // Get the total count of messages (for pagination purposes)
                string countQuery = @"
            SELECT COUNT(*) 
            FROM Inquiries m
            INNER JOIN Properties p ON p.PropertyId = m.PropertyID
            WHERE p.UserID = @UserId";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    countQuery += " AND m.Message LIKE @SearchTerm";
                }

                if (filter == "unread")
                {
                    countQuery += " AND m.Status = 'Unread'";
                }
                else if (filter == "read")
                {
                    countQuery += " AND m.Status = 'Read'";
                }

                int totalMessages = 0;
                using (SqlCommand cmdCount = new SqlCommand(countQuery, con))
                {
                    cmdCount.Parameters.AddWithValue("@UserId", userId);
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        cmdCount.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                    }

                    totalMessages = (int)await cmdCount.ExecuteScalarAsync();
                }

                // Set pagination details on the ViewModel
                var messageListViewModel = new PropertyMessageViewModel
                {
                    PagedMessages = messages,
                    CurrentPage = page,
                    TotalMessages = totalMessages,
                    MessagesPerPage = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalMessages / pageSize),
                    SearchTerm = searchTerm,
                    Filter = filter
                };

                return View(messageListViewModel);
            }
        }

        [HttpGet]
     public IActionResult Total_Properties()
{
    var model = new SuperAdminDashboardViewModel();
    model.Properties = new List<PropertyViewModel>();

    string connectionString = _configuration.GetConnectionString("DefaultConnection");

    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        conn.Open();

        // 1. Fetch Property Details
        string propertyQuery = @"
            SELECT 
                p.PropertyId, p.Title, p.Price, p.Address, p.ImagePaths, p.IsAvailable,
                u.name AS AddedBy,
                (SELECT COUNT(*) FROM Reviews r WHERE r.PropertyID = p.PropertyId) AS TotalReviews,
                (SELECT COUNT(*) FROM Inquiries i WHERE i.PropertyID = p.PropertyId) AS TotalInquiries
            FROM Properties p
            INNER JOIN users u ON p.UserId = u.user_id";

        using (SqlCommand cmd = new SqlCommand(propertyQuery, conn))
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                model.Properties.Add(new PropertyViewModel
                {
                    Id = Convert.ToInt32(reader["PropertyId"]),
                    Title = reader["Title"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    Address = reader["Address"].ToString(),
                    ImageUrl = reader["ImagePaths"]?.ToString()?.Split(',')?.FirstOrDefault(),
                    AddedBy = reader["AddedBy"].ToString(),
                    TotalReviews = Convert.ToInt32(reader["TotalReviews"]),
                    TotalInquiries = Convert.ToInt32(reader["TotalInquiries"]),
                    Status = reader["IsAvailable"] != DBNull.Value && Convert.ToBoolean(reader["IsAvailable"]) ? "Available" : "Unavailable"



                });
            }
        }

                // 2. Fetch Counts
                string countQuery = @"
    SELECT 
        (SELECT COUNT(*) FROM Properties) AS TotalProperties,
        (SELECT COUNT(*) FROM Properties WHERE IsAvailable = 1) AS ActiveListings,
        (SELECT COUNT(*) FROM Inquiries) AS TotalInquiries";

                using (SqlCommand countCmd = new SqlCommand(countQuery, conn))
        using (SqlDataReader countReader = countCmd.ExecuteReader())
        {
            if (countReader.Read())
            {
                model.TotalProperties = Convert.ToInt32(countReader["TotalProperties"]);
                model.ActiveListings = Convert.ToInt32(countReader["ActiveListings"]);
                model.TotalInquiries = Convert.ToInt32(countReader["TotalInquiries"]);
            }
        }
    }

    return View(model);
}

        [HttpPost]
public IActionResult Total_Properties(int id)
{
    string connectionString = _configuration.GetConnectionString("DefaultConnection");

    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        string query = "DELETE FROM Properties WHERE PropertyId = @Id";

        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rowsAffected > 0)
            {
                TempData["Success"] = "Property deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete property.";
            }
        }
    }

    return RedirectToAction("Total_Properties"); // Redirect back to property list view
}
        public async Task<IActionResult> Particular_property(int id)
        {
            ViewData["ActivePage"] = "Particular_property";

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }

            var propertyDetails = new PropertyDetailsViewModel
            {
                Reviews = new List<ReviewModel>(),
                Inquiries = new List<InquiryModel>()
            };

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

                // Fetch Property Details
                string query = @"
            SELECT p.PropertyId, p.Title, p.Address, p.Price, p.ImagePaths,
                   u.Name, u.Email, u.Mobile
            FROM Properties p
            INNER JOIN Users u ON u.user_id = p.UserId
            WHERE p.PropertyId = @PropertyId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PropertyId", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            propertyDetails.PropertyId = reader["PropertyId"].ToString();
                            propertyDetails.Title = reader["Title"].ToString();
                            propertyDetails.Address = reader["Address"].ToString();
                            propertyDetails.Price = Convert.ToDecimal(reader["Price"]);
                            propertyDetails.ImageUrl = reader["ImagePaths"].ToString()?.Split(',').FirstOrDefault();
                            propertyDetails.OwnerName = reader["Name"].ToString();
                            propertyDetails.OwnerEmail = reader["Email"].ToString();
                            propertyDetails.OwnerMobile = reader["Mobile"].ToString();
                        }
                    }
                }

                // Fetch Reviews
                string reviewQuery = @"
            SELECT r.ReviewId, r.Comment, r.Rating, r.ReviewDate, u.Name AS ReviewerName, p.Title
            FROM Reviews r
            INNER JOIN Users u ON u.user_id = r.UserId
            INNER JOIN Properties p ON p.PropertyId = r.PropertyId
            WHERE r.PropertyId = @PropertyId";

                using (SqlCommand cmd = new SqlCommand(reviewQuery, con))
                {
                    cmd.Parameters.AddWithValue("@PropertyId", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            propertyDetails.Reviews.Add(new ReviewModel
                            {
                                ReviewId = Convert.ToInt32(reader["ReviewId"]),
                                PropertyTitle = reader["Title"].ToString(),
                                ReviewerName = reader["ReviewerName"].ToString(),
                                Date = Convert.ToDateTime(reader["ReviewDate"]),
                                Rating = Convert.ToInt32(reader["Rating"]),
                                Comment = reader["Comment"].ToString()
                            });
                        }
                    }
                }

                // Fetch Inquiries
                string inquiryQuery = @"
            SELECT 
                i.InquiryId, 
                u.Name, 
                u.Email, 
                u.Mobile, 
                i.Message
            FROM 
                Inquiries i
            INNER JOIN 
                Users u ON i.UserId = u.user_id
            WHERE 
                i.PropertyId = @PropertyId";

                using (SqlCommand cmd = new SqlCommand(inquiryQuery, con))
                {
                    cmd.Parameters.AddWithValue("@PropertyId", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            propertyDetails.Inquiries.Add(new InquiryModel
                            {
                                InquiryId = Convert.ToInt32(reader["InquiryId"]),
                                GuestName = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Contact = reader["Mobile"].ToString(),
                                Message = reader["Message"].ToString()
                            });
                        }
                    }
                }
            }

            return View("Particular_property", propertyDetails); ;
        }
        [HttpPost]
        public IActionResult inquiries(int inquiryId, int propertyId)
        {
            // Define the connection string (ensure to replace with your actual connection string)
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Define the query to delete the inquiry
            string query = "DELETE FROM Inquiries WHERE InquiryId = @InquiryId";

            // Use ADO.NET to execute the query
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@InquiryId", inquiryId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // Handle exceptions (logging, etc.)
                    ViewBag.ErrorMessage = "Error deleting inquiry: " + ex.Message;
                }
            }

            // Redirect to the property details page after deletion
            return RedirectToAction("inquiries", new { id = propertyId });
        }
        [HttpDelete]
        public IActionResult DeleteReview(int reviewId, int propertyId)
        {
            // Define the connection string (ensure to replace with your actual connection string)
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Define the query to delete the review
            string query = "DELETE FROM Reviews WHERE ReviewId = @ReviewId";

            // Use ADO.NET to execute the query
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReviewId", reviewId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // Handle exceptions (logging, etc.)
                    ViewBag.ErrorMessage = "Error deleting review: " + ex.Message;
                }
            }

            // Redirect to the property details page after deletion
            return RedirectToAction("Particular_property", new { id = propertyId });
        }
        //public IActionResult AdminProfile() {

        //    var redirect = RedirectToLoginIfNotLoggedIn();
        //    if (redirect != null) return redirect;

        //    return View();
        //}

        [HttpGet]
        public IActionResult AdminProfile()
        {
            ViewData["Title"] = "AdminProfile";
            ViewData["ActivePage"] = "AdminProfile";
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            AdminProfile model = new AdminProfile();

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
        public IActionResult AdminProfile(AdminProfile model, IFormFile ProfilePicFile)
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
            return View("Index", "Home");
        }


        private void FillUserProfileData(AdminProfile model)
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


        public IActionResult Payment_Details()
        {
            var payments = new List<PaymentDisplayViewModel>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = @"
            SELECT 
                p.PaymentId,
                p.OrderId,
                pr.Title AS PropertyTitle,    
                u.name AS UserName,           
                p.PaymentGateway,
                p.Amount,
                p.PaymentStatus,
         
                p.RazorpayPaymentId,
      
                p.PaymentDate
            FROM Payments p
            INNER JOIN Properties pr ON p.PropertyId = pr.PropertyId
            INNER JOIN users u ON p.UserId = u.user_id
            ORDER BY p.PaymentDate DESC;";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    payments.Add(new PaymentDisplayViewModel
                    {
                        PaymentId = Convert.ToInt32(reader["PaymentId"]),
                        OrderId = reader["OrderId"].ToString(),
                        PropertyTitle = reader["PropertyTitle"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        PaymentGateway = reader["PaymentGateway"].ToString(),
                        Amount = Convert.ToDecimal(reader["Amount"]),
                        PaymentStatus = reader["PaymentStatus"].ToString(),
                       
                        RazorpayPaymentId = reader["RazorpayPaymentId"].ToString(),
                      
                        PaymentDate = Convert.ToDateTime(reader["PaymentDate"])
                    });
                }
            }

            return View(payments);
        }


    }
}
