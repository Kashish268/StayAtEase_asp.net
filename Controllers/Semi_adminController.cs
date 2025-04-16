using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Semi_adminController : BaseController
    {
        private readonly IConfiguration _configuration;

        public Semi_adminController(IConfiguration configuration) : base(configuration)
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
                //ReviewsPerPage = pageSize,
                SearchTerm = searchTerm ?? ""
            };

            return View("Reviews", viewModel); // <-- Ensure the view name matches your .cshtml file (case-sensitive on Linux)
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

                // Adjust query to only use existing columns
                string query = @"
        SELECT PropertyId, Title, Price, SquareFootage, Address, ImagePaths 
        FROM Properties 
        WHERE UserId = @UserId";

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query += " AND Title LIKE @SearchTerm";
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
                                ImageUrl = reader["ImagePaths"].ToString()?.Split(',').FirstOrDefault()

                                // IsAvailable removed because column doesn't exist
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
                TotalInquiries = 0,
                SearchTerm = searchTerm ?? "",
                Filter = filter
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult MyProfile()
        {
            ViewData["Title"] = "My Profile";
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            ProfileDetailsViewModel model = new ProfileDetailsViewModel();

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
        public IActionResult MyProfile(ProfileDetailsViewModel model, IFormFile ProfilePicFile)
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


        private void FillUserProfileData(ProfileDetailsViewModel model)
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
