
using Microsoft.AspNetCore.Mvc;
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

        public Super_AdminController(IConfiguration configuration, ILogger<Super_AdminController> logger)
            : base(configuration)
        {
            _config = configuration;
            _logger = logger;
        }
//        [HttpGet]
//        public IActionResult Super_AdminDashboard()
//        {
//            ViewData["ActivePage"] = "Super_AdminDashboard";
//            int? userId = HttpContext.Session.GetInt32("UserId");

//            if (userId == null)
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            var model = new SuperAdminDashboardViewModel
//            {
//                Tenants = new List<User>(),
//                PropertyOwners = new List<User>(),
//                LatestMessages = new List<PropertyMessageViewModel>()
//            };

//            string connectionString = _config.GetConnectionString("DefaultConnection");

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                // Example logic for filling the model data
//                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Properties", conn))
//                {
//                    model.TotalProperties = (int)cmd.ExecuteScalar();
//                }
//                // Active Listings
//                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Properties WHERE IsAvailable = 1", conn))
//                {
//                    model.ActiveListings = (int)cmd.ExecuteScalar();
//                }

//                // Total Inquiries
//                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Inquiries", conn))
//                {
//                    model.TotalInquiries = (int)cmd.ExecuteScalar();
//                }

//                // Average Review Rating
//                using (SqlCommand cmd = new SqlCommand("SELECT AVG(CAST(Rating AS FLOAT)) FROM Reviews", conn))
//                {
//                    var result = cmd.ExecuteScalar();
//                    model.AverageReviewRating = result != DBNull.Value ? Convert.ToDouble(result) : 0;
//                }

//                // Tenants
//                using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 user_id, name, mobile, email, role, status, created_at, profile_pic FROM users WHERE role = 'tenant'", conn))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        model.Tenants.Add(new User
//                        {
//                            UserId = reader.GetInt32(0),
//                            Name = reader.GetString(1),
//                            Mobile = reader.GetString(2),
//                            Email = reader.GetString(3),
//                            Role = reader.GetString(4),
//                            Status = reader.GetString(5),
//                            CreatedAt = reader.GetDateTime(6),
//                            ProfilePic = reader.IsDBNull(7) ? null : reader.GetString(7)
//                        });
//                    }
//                }

//                // Property Owners
//                using (SqlCommand cmd = new SqlCommand("SELECT TOP 5 user_id, name, mobile, email, role, status, created_at, profile_pic FROM users WHERE role = 'roomowner'", conn))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        model.PropertyOwners.Add(new User
//                        {
//                            UserId = reader.GetInt32(0),
//                            Name = reader.GetString(1),
//                            Mobile = reader.GetString(2),
//                            Email = reader.GetString(3),
//                            Role = reader.GetString(4),
//                            Status = reader.GetString(5),
//                            CreatedAt = reader.GetDateTime(6),
//                            ProfilePic = reader.IsDBNull(7) ? null : reader.GetString(7)
//                        });
//                    }
//                }

//                // Latest Messages
//                string messageQuery = @"
//                SELECT TOP 3 u.name, u.email, u.mobile, p.PropertyId, i.Message
//                FROM Inquiries i
//                JOIN users u ON i.UserID = u.user_id
//                JOIN Properties p ON i.PropertyID = p.PropertyId
//                ORDER BY i.InquiryDate DESC";

//                using (SqlCommand cmd = new SqlCommand(messageQuery, conn))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                      model.LatestMessages.Add(new PropertyMessageViewModel
//{
//    Name = reader.GetString(0),
//    Email = reader.GetString(1),
//    Contact = reader.GetString(2),
//    PropertyId = "P-" + reader.GetInt32(3),
//    Message = reader.GetString(4)
//});

//                    }
//                }
//            }

//           return View();

//        }

        public IActionResult Total_User()
        {
            ViewData["ActivePage"] = "Total_User";

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;


            List<User> tenants = new List<User>();
            string connectionString = _config.GetConnectionString("DefaultConnection");

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
            string connectionString = _config.GetConnectionString("DefaultConnection");

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
            string connectionString = _config.GetConnectionString("DefaultConnection");

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
            string connectionString = _config.GetConnectionString("DefaultConnection");

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
            string connectionString = _config.GetConnectionString("DefaultConnection");

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
            string connectionString = _config.GetConnectionString("DefaultConnection");

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
            string connectionString = _config.GetConnectionString("DefaultConnection");

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
            List<PropertyViewModel> properties = new List<PropertyViewModel>();
            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            p.PropertyId, p.Title, p.Price, p.Address, p.ImagePaths, 
            u.name AS AddedBy,
            (SELECT COUNT(*) FROM Reviews r WHERE r.PropertyID = p.PropertyId) AS TotalReviews,
            (SELECT COUNT(*) FROM Inquiries i WHERE i.PropertyID = p.PropertyId) AS TotalInquiries
        FROM Properties p
        INNER JOIN users u ON p.UserId = u.user_id";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    properties.Add(new PropertyViewModel
                    {
                        Id = Convert.ToInt32(reader["PropertyId"]),
                        Title = reader["Title"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Address = reader["Address"].ToString(),
                        ImageUrl = reader["ImagePaths"]?.ToString()?.Split(',')?.FirstOrDefault(), // if multiple images
                        AddedBy = reader["AddedBy"].ToString(),
                        TotalReviews = Convert.ToInt32(reader["TotalReviews"]),
                        TotalInquiries = Convert.ToInt32(reader["TotalInquiries"]),
                        Status = "Active" // you can later modify this with actual data
                    });
                }
            }

            return View(properties);
        }
        [HttpPost]
        public IActionResult Total_Properties(int id)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");

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

        public IActionResult Particular_property()
        {
            ViewData["ActivePage"] = "Total_Properties";
            return View();
        }
        public IActionResult AdminProfile()
        {

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;


            var model = new ProfileDetailsViewModel
            {
                ProfileImageUrl = "/profile.png",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "+1 (555) 000-0000",
                Location = "San Francisco, CA",
                Timezone = "(GMT-08:00) Pacific Time",
                Bio = "Property dealer"
            };
            return View(model);
        }
    }
}