using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApplication1.Controllers
{
    public class Semi_adminController : BaseController
    {
        private readonly IConfiguration _configuration;

        public Semi_adminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Dashboard Action
        public IActionResult Dashboard()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            
            return View();
        }

        // Add Property (GET) Action
        [HttpGet]
        public IActionResult Add_Properties()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }

        // Create Property (POST) Action
        [HttpPost]
        public async Task<IActionResult> Create(AddPropertyModel model)
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;
            int? userId = HttpContext.Session.GetInt32("UserId");

            // Ensure the user_id exists in the session before proceeding
            if (userId==null)
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }
            ViewBag.UserId = userId;
            if (!ModelState.IsValid)
                return View(model);

            var amenities = Request.Form["Amenities"];
            string amenitiesString = string.Join(", ", amenities);

            var imagePaths = new List<string>();
            if (model.Images != null && model.Images.Any())
            {
                foreach (var image in model.Images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "properties", fileName);

                        var directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        imagePaths.Add("/images/properties/" + fileName);
                    }
                }
            }

            string imagePathString = string.Join(", ", imagePaths);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                INSERT INTO Properties 
                (Title, Price, SquareFootage, Address, Bedrooms, Bathrooms, PropertyType, Amenities, ImagePaths, UserId)
                VALUES
                (@Title, @Price, @SquareFootage, @Address, @Bedrooms, @Bathrooms, @PropertyType, @Amenities, @ImagePaths, @UserId)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Title", model.Title);
                        cmd.Parameters.AddWithValue("@Price", model.Price);
                        cmd.Parameters.AddWithValue("@SquareFootage", model.SquareFootage);
                        cmd.Parameters.AddWithValue("@Address", model.Address);
                        cmd.Parameters.AddWithValue("@Bedrooms", model.Bedrooms);
                        cmd.Parameters.AddWithValue("@Bathrooms", model.Bathrooms);
                        cmd.Parameters.AddWithValue("@PropertyType", model.PropertyType);
                        cmd.Parameters.AddWithValue("@Amenities", amenitiesString);
                        cmd.Parameters.AddWithValue("@ImagePaths", imagePathString);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        con.Open();
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            ModelState.AddModelError("", "Failed to add the property.");
                            return View(model);
                        }
                    }
                }

                TempData["Success"] = "Property added successfully!";
                return RedirectToAction("Property_List", "Semi_admin");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding property: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while adding the property. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        // Reviews Action
        public async Task<IActionResult> Reviews(string? searchTerm, int page = 1)
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }

            var reviews = new List<PropertyReview>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            int pageSize = 10;
            int offset = (page - 1) * pageSize;
            int totalReviewsCount = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

                string query = @"
            SELECT r.ReviewID, u.Name, r.ReviewDate, r.Rating, r.Comment, p.PropertyID, u.profile_pic
            FROM Reviews r
            INNER JOIN Properties p ON p.PropertyID = r.PropertyID
            INNER JOIN Users u ON u.user_id = r.UserID
            WHERE p.UserId = @UserId";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND r.Comment LIKE @SearchTerm";
                }

                query += " ORDER BY r.ReviewDate DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    if (!string.IsNullOrEmpty(searchTerm))
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reviews.Add(new PropertyReview
                            {
                                Id = Convert.ToInt32(reader["ReviewID"]),
                                Name = reader["Name"].ToString(),
                                Property = reader["PropertyID"].ToString(),
                                Rating = Convert.ToInt32(reader["Rating"]),
                                ReviewText = reader["Comment"].ToString(),
                                ImageUrl = reader["profile_pic"] != DBNull.Value ? reader["profile_pic"].ToString() : string.Empty,
                                Date = reader["ReviewDate"] != DBNull.Value ? Convert.ToDateTime(reader["ReviewDate"]).ToString("yyyy-MM-dd") : string.Empty,
                            });
                        }
                    }
                }

                // Get total count
                string countQuery = @"
            SELECT COUNT(*) FROM Reviews r
            INNER JOIN Properties p ON p.PropertyID = r.PropertyID
            WHERE p.UserId = @UserId";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    countQuery += " AND r.Comment LIKE @SearchTerm";
                }

                using (SqlCommand countCmd = new SqlCommand(countQuery, con))
                {
                    countCmd.Parameters.AddWithValue("@UserId", userId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        countCmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    totalReviewsCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                }
            }

            var viewModel = new PropertyReviewViewModel
            {
                Reviews = reviews,
                TotalReviews = totalReviewsCount,
                CurrentPage = page,
                ReviewsPerPage = pageSize,
                SearchTerm = searchTerm ?? ""
            };

<<<<<<< HEAD
            return View("Reviews", viewModel); 
=======
            return View("Reviews", viewModel); // <-- Ensure the view name matches your .cshtml file (case-sensitive on Linux)
>>>>>>> ef7b32b2840d9e0db7a201e1360e94cef2caa37c
        }




        // Property List Action



        [HttpGet]
        public async Task<IActionResult> Messages(string? searchTerm, int page = 1, string filter = "all")
        {
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



        // Messages Action



        // Profile Action
        [HttpGet]
        public async Task<IActionResult> Property_List(string? searchTerm, int page = 1, string filter = "all")
        {
           

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;
            int? userId = HttpContext.Session.GetInt32("UserId");

            // Ensure the user_id exists in the session before proceeding
            if (userId == null)
            {
                TempData["Error"] = "User not logged in.";
                return RedirectToAction("Login", "Account");
            }
            ViewBag.UserId = userId;

            var properties = new List<PropertyViewModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();

                string query = @"SELECT * FROM Properties WHERE UserId = @UserId";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND Title LIKE @SearchTerm";
                }

                if (filter == "available")
                {
                    query += " AND IsAvailable = 1";
                }
                else if (filter == "unavailable")
                {
                    query += " AND IsAvailable = 0";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    if (!string.IsNullOrEmpty(searchTerm))
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            properties.Add(new PropertyViewModel
                            {
                                Id = Convert.ToInt32(reader["PropertyId"]),
                                Title = reader["Title"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Area = Convert.ToInt32(reader["SquareFootage"]),
                                Address = reader["Address"].ToString(),
                                IsAvailable = reader["IsAvailable"] != DBNull.Value && Convert.ToBoolean(reader["IsAvailable"]),
                                ImageUrl = reader["ImagePaths"].ToString()?.Split(',').FirstOrDefault()
                            });
                        }
                    }
                }
            }

            int currentPage = page;
            int pageSize = 8;
            var pagedProperties = properties.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new MyListingsViewModel
            {
                Properties = pagedProperties,
                CurrentPage = currentPage,
                TotalInquiries = 0, // Optional: Add inquiry logic here
                SearchTerm = searchTerm ?? "",
                Filter = filter
            };

            return View(viewModel);
        }




        public IActionResult MyProfile()
        {
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

        // POST: MyProfile
       


        // Property Details Action
        public IActionResult Property_Details()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }

        // Edit Property Action
        public IActionResult Edit_Property()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }
    }
}
