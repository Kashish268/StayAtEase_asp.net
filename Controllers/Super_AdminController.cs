
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Super_AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        public IActionResult Super_AdminDashboard()
        {
            ViewData["ActivePage"] = "Super_AdminDashboard";


            return View();
        }

        public IActionResult Total_User()
        {
            ViewData["ActivePage"] = "Total_User";
            return View();
        }
        public IActionResult Total_Admin()
        {
            ViewData["ActivePage"] = "Total_Admin";

            // Fetch RoomOwner users from the database
        

            // Pass the data to the view
            return View();
        }

        public IActionResult Property_Reviews()
        {
            ViewData["ActivePage"] = "Property_Reviews";
            return View();
        }
        public async Task<IActionResult> Inquiries(string? searchTerm, int page = 1, string filter = "all")
        {
            ViewData["ActivePage"] = "Inquiries";

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }

            // Pagination settings
            int pageSize = 5; // Number of messages per page
            int skip = (page - 1) * pageSize;

            var messages = new List<PropertyMessageViewModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

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
            WHERE p.UserID = @UserId";

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
                                Name = reader["name"].ToString(),
                                PropertyId = reader["PropertyID"].ToString(),
                                Email = reader["email"].ToString(),
                                Contact = reader["mobile"].ToString(),
                                Message = reader["Message"].ToString(),
                             
                                ImageUrl = reader["Profile_pic"].ToString()?.Split(',').FirstOrDefault()
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
             
                };

                return View(messageListViewModel);
            }
        }


        public IActionResult Total_Properties()
        {
            ViewData["ActivePage"] = "Total_Properties";
            return View();
        }

        public IActionResult Particular_property()
        {
            ViewData["ActivePage"] = "Total_Properties";
            return View();
        }
        public IActionResult AdminProfile() {
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
