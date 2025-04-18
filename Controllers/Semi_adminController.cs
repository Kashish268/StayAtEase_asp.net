using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Semi_adminController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Semi_adminController(IConfiguration configuration, IWebHostEnvironment env) : base(configuration)
        {
            _configuration = configuration;
            _env = env;

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
      
        [HttpPost]
        public async Task<IActionResult> Add_Properties(AddPropertyModel model)
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
            if (!ModelState.IsValid)
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }


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
            SELECT r.ReviewID, u.Name, r.ReviewDate, r.Rating, r.Comment, p.PropertyID,p.Title, u.profile_pic
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
                                PropertyTitle = reader["Title"].ToString(),
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
                            var rawImagePaths = reader["ImagePaths"]?.ToString();
                            var imagePaths = !string.IsNullOrEmpty(rawImagePaths)
                                ? rawImagePaths.Split(',').Select(p => p.Trim()).ToList()
                                : new List<string>();

                           bool hasIsAvailable = Enumerable.Range(0, reader.FieldCount)
                                .Select(reader.GetName)
                                .Contains("IsAvailable", StringComparer.OrdinalIgnoreCase);

properties.Add(new PropertyViewModel
{
    Id = Convert.ToInt32(reader["PropertyId"]),
    Title = reader["Title"].ToString(),
    Price = Convert.ToDecimal(reader["Price"]),
    Area = Convert.ToInt32(reader["SquareFootage"]),
    Address = reader["Address"].ToString(),
    IsAvailable = hasIsAvailable && !reader.IsDBNull(reader.GetOrdinal("IsAvailable")) && reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
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

        // POST: MyProfile



        // Property Details Action
        public async Task<IActionResult> Property_Details(int id)
        {
            ViewData["ActivePage"] = "PropertyDetails";

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

                // 🏠 Fetch Property Details (Updated with more fields)
                string query = @"
        SELECT p.PropertyId, p.Title, p.Address, p.Price, p.ImagePaths,
               p.SquareFootage, p.Bedrooms, p.Bathrooms, p.PropertyType,
               u.name, u.Email, u.Mobile
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

                            propertyDetails.SquareFootage = Convert.ToInt32(reader["SquareFootage"]);
                            propertyDetails.Bedrooms = Convert.ToInt32(reader["Bedrooms"]);
                            propertyDetails.Bathrooms = Convert.ToInt32(reader["Bathrooms"]);
                            propertyDetails.PropertyType = reader["PropertyType"].ToString();

                            propertyDetails.OwnerName = reader["name"].ToString();
                            propertyDetails.OwnerEmail = reader["email"].ToString();
                            propertyDetails.OwnerMobile = reader["mobile"].ToString();
                        }
                    }
                }

                // ⭐ Fetch Reviews
                string reviewQuery = @"
        SELECT r.ReviewId, r.Comment, r.Rating, r.ReviewDate, u.name AS ReviewerName, p.Title
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

                propertyDetails.AverageRating = propertyDetails.Reviews.Any()
                    ? propertyDetails.Reviews.Average(r => r.Rating)
                    : 0;

                // 💬 Fetch Inquiries
                string inquiryQuery = @"
        SELECT i.InquiryId, u.Name, u.email, u.mobile, i.Message
        FROM Inquiries i
        INNER JOIN Users u ON i.UserId = u.user_id
        WHERE i.PropertyId = @PropertyId";

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
                                Email = reader["email"].ToString(),
                                Contact = reader["mobile"].ToString(),
                                Message = reader["Message"].ToString()
                            });
                        }
                    }
                }
            }

            return View(propertyDetails);
        }



        public IActionResult DeleteInquiry(int inquiryId, int propertyId)
        {
            // Define the connection string (ensure to replace with your actual connection string)
            string connectionString = "YourConnectionStringHere";

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
            return RedirectToAction("Property_Details", new { id = propertyId });
        }
        [HttpDelete]
        public IActionResult DeleteReview(int reviewId, int propertyId)
        {
            // Define the connection string (ensure to replace with your actual connection string)
            string connectionString = "YourConnectionStringHere";

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
            return RedirectToAction("Property_Details", new { id = propertyId });
        }
        // Edit Property Action
        [HttpGet]
        public IActionResult Edit_Property(int id)
        {
            var model = new AddPropertyModel();
            model.PropertyId = id; // ensure the ID is carried over to the view

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = "SELECT * FROM Properties WHERE PropertyId = @PropertyId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PropertyId", id);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model.Title = reader["Title"]?.ToString();
                        model.Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0;
                        model.SquareFootage = reader["SquareFootage"] != DBNull.Value ? Convert.ToInt32(reader["SquareFootage"]) : 0;
                        model.Address = reader["Address"]?.ToString();
                        model.Bedrooms = reader["Bedrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bedrooms"]) : 0;
                        model.Bathrooms = reader["Bathrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bathrooms"]) : 0;
                        model.PropertyType = reader["PropertyType"]?.ToString();
                        model.UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : 0;

                        var amenitiesStr = reader["Amenities"]?.ToString();
                        if (!string.IsNullOrEmpty(amenitiesStr))
                        {
                            model.Amenities = amenitiesStr.Split(',').Select(a => a.Trim()).ToList();
                        }

                        var images = reader["ImagePaths"]?.ToString();
                        if (!string.IsNullOrEmpty(images))
                        {
                            model.ExistingImages = images.Split(',').Select(i => i.Trim()).ToList();
                            Console.WriteLine("Images string from DB: " + images); // or use logging

                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Edit_Property(int id, AddPropertyModel model, List<IFormFile> Images, string[] Amenities)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");
                    }
                }
               
            }

            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "properties");

            // Ensure the upload folder exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Convert amenities array to string
            string amenitiesString = string.Join(", ", Amenities);

            // Upload new images
            var uploadedImagePaths = new List<string>();
            if (Images != null && Images.Any())
            {
                foreach (var image in Images)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        uploadedImagePaths.Add("/images/properties/" + fileName);
                    }
                }
            }

            // Merge old and new image paths
            string oldImages = string.Join(", ", model.ExistingImages ?? new List<string>());
            string newImages = string.Join(", ", uploadedImagePaths);
            string imagePathString = string.IsNullOrEmpty(newImages)
                ? oldImages
                : string.IsNullOrEmpty(oldImages)
                    ? newImages
                    : oldImages + ", " + newImages;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                UPDATE Properties SET 
                    Title = @Title,
                    Price = @Price,
                    SquareFootage = @SquareFootage,
                    Address = @Address,
                    Bedrooms = @Bedrooms,
                    Bathrooms = @Bathrooms,
                    PropertyType = @PropertyType,
                    Amenities = @Amenities,
                    ImagePaths = @ImagePaths
                WHERE PropertyId = @PropertyId";

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
                        cmd.Parameters.AddWithValue("@PropertyId", id);

                        con.Open();
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            ModelState.AddModelError("", "Failed to update the property.");
                            return View(model);
                        }
                    }
                }

                TempData["Success"] = "Property updated successfully!";
                return RedirectToAction("Property_Details", new { id = id });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating property: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the property. Please try again.");
                return View(model);
            }
        }




        // Action to handle image deletion
        public IActionResult DeleteImage(int id, string imagePath)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "/images/properties/");
            if (!string.IsNullOrEmpty(imagePath))
            {
                string fullImagePath = Path.Combine(uploadsFolder, Path.GetFileName(imagePath.TrimStart('/')));

                if (System.IO.File.Exists(fullImagePath))
                {
                    System.IO.File.Delete(fullImagePath);
                }
            }
            else
            {
                Console.WriteLine("imagePath is null or empty. Cannot delete image.");
            }

            // Now, remove image from the database
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string deleteImageQuery = @"UPDATE Properties 
                                    SET ImagePaths = REPLACE(ImagePaths, @ImagePath, '') 
                                    WHERE PropertyId = @PropertyId";

                SqlCommand cmd = new SqlCommand(deleteImageQuery, conn);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                cmd.Parameters.AddWithValue("@PropertyId", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Redirect to the Property Details page after deletion
            return RedirectToAction("Property_Details", new { id = id });
        }


    }
}
