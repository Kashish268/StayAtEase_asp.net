using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication1.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

            var model = new DashboardViewModel
            {
                TotalProperties = 24,
                ActiveListings = 18,
                TotalInquiries = 156,
                ReviewRating = 4.8,
                Messages = new List<PropertyMessageViewModel>
                {
                    new PropertyMessageViewModel { Name = "Sarah Johnson", PropertyId = "P_101", Email = "sarahjohnson@gmail.com", Contact = "1236547890", Message = "Are there any restrictions on lease agreements?" },
                    new PropertyMessageViewModel { Name = "Michael Brown", PropertyId = "P_102", Email = "michaelbrown@gmail.com", Contact = "9874563210", Message = "Are there any restrictions on lease agreements?" },
                    new PropertyMessageViewModel { Name = "Emma Davis", PropertyId = "P_103", Email = "emmadavis@gmail.com", Contact = "7410258963", Message = "What documents are required for booking?" }
                },
                Reviews = new List<PropertyReview>
                {
                    new PropertyReview { Name = "John Smith", Date = "Feb 10, 2024", Rating = 5, Text = "Amazing property with stunning views. Highly recommended!", Property = "Lakefront Cottage", ImageUrl = "/images/user1.jpg" },
                    new PropertyReview { Name = "Lisa Anderson", Date = "Feb 8, 2024", Rating = 4, Text = "Great location and comfortable stay. Would visit again.", Property = "Downtown Loft", ImageUrl = "/images/user2.jpg" },
                    new PropertyReview { Name = "David Wilson", Date = "Feb 7, 2024", Rating = 5, Text = "Perfect getaway spot. Everything was exactly as described.", Property = "Mountain View Cabin", ImageUrl = "/images/user3.jpg" }
                }
            };
            return View("Dashboard", model);
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


        // Reviews Action
        public IActionResult Reviews()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            return View();
        }

        // Property List Action
        public IActionResult Property_List(int currentPage = 1, int pageSize = 8)
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            var properties = new List<PropertyViewModel>
            {
                new PropertyViewModel { Id = 1, Title = "Luxury Apartment", Price = "25000", Area = 1200, Address = "Downtown, City", IsAvailable = true, ImageUrl = "/assets/Property1.jpg" },
                new PropertyViewModel { Id = 2, Title = "Modern Villa", Price = "50000", Area = 2000, Address = "Uptown, City", IsAvailable = false, ImageUrl = "/assets/Property2.jpg" },
                new PropertyViewModel { Id = 3, Title = "Luxury Apartment", Price = "25000", Area = 1200, Address = "Downtown, City", IsAvailable = true, ImageUrl = "/assets/Property1.jpg" },
                new PropertyViewModel { Id = 4, Title = "Modern Villa", Price = "50000", Area = 2000, Address = "Uptown, City", IsAvailable = false, ImageUrl = "/assets/Property2.jpg" }
            };

            var totalProperties = properties.Count;

            var model = new MyListingsViewModel
            {
                TotalProperties = totalProperties,
                ActiveListings = properties.Count(p => p.IsAvailable),
                TotalInquiries = 25,
                CurrentPage = currentPage,
                PageSize = pageSize,
                Properties = properties.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
            };

            return View(model);
        }

        // Messages Action
        public IActionResult Messages()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            ViewData["ActivePage"] = "Messages";
            return View();
        }

        // Profile Action
        // GET: MyProfile
        [HttpGet]
        public IActionResult MyProfile()
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var repo = new UserRepository(_configuration);
            var model = repo.GetUserProfile(userId.Value);
            return View(model);
        }

        // POST: MyProfile
        [HttpPost]
        public async Task<IActionResult> MyProfile(ProfileDetailsViewModel model)
        {
            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            // Handle profile image upload if a file was posted
            var file = Request.Form.Files["ProfileImage"];
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                model.ProfileImageUrl = "/images/profiles/" + fileName;
            }

            model.UserId = userId.Value;
            var repo = new UserRepository(_configuration);
            bool updated = repo.UpdateUserProfile(model);

            if (updated)
                TempData["Success"] = "Profile updated successfully!";
            else
                TempData["Error"] = "Failed to update profile.";

            return View(model);
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
