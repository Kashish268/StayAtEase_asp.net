using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace WebApplication1.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IConfiguration _configuration;

        public WishlistController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: /Wishlist/Toggle
        [HttpPost]
        public JsonResult Toggle([FromBody] WishlistToggleModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Please login to add in wishlist." });
            }

            bool isWishlisted = false;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if item is already in wishlist
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Wishlist WHERE UserId = @UserId AND PropertyId = @PropertyId", conn);
                checkCmd.Parameters.AddWithValue("@UserId", userId.Value);
                checkCmd.Parameters.AddWithValue("@PropertyId", model.PropertyId);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    // Remove from wishlist
                    SqlCommand deleteCmd = new SqlCommand("DELETE FROM Wishlist WHERE UserId = @UserId AND PropertyId = @PropertyId", conn);
                    deleteCmd.Parameters.AddWithValue("@UserId", userId.Value);
                    deleteCmd.Parameters.AddWithValue("@PropertyId", model.PropertyId);
                    deleteCmd.ExecuteNonQuery();
                    isWishlisted = false;
                }
                else
                {
                    // Add to wishlist
                    SqlCommand insertCmd = new SqlCommand("INSERT INTO Wishlist (UserId, PropertyId) VALUES (@UserId, @PropertyId)", conn);
                    insertCmd.Parameters.AddWithValue("@UserId", userId.Value);
                    insertCmd.Parameters.AddWithValue("@PropertyId", model.PropertyId);
                    insertCmd.ExecuteNonQuery();
                    isWishlisted = true;
                }
            }

            return Json(new { success = true, isWishlisted = isWishlisted });
        }

        // GET: /Wishlist/List
        public IActionResult List()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // or wherever your login page is
            }

            List<WishlistItem> wishlistItems = new List<WishlistItem>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT P.PropertyId, P.PropertyName, P.ImageUrl, P.Price
                    FROM Wishlist W
                    JOIN Property P ON W.PropertyId = P.PropertyId
                    WHERE W.UserId = @UserId", conn);

                cmd.Parameters.AddWithValue("@UserId", userId.Value);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    wishlistItems.Add(new WishlistItem
                    {
                        PropertyId = reader.GetInt32(0),
                        PropertyName = reader.GetString(1),
                        ImageUrl = reader.GetString(2),
                        Price = reader.GetDecimal(3)
                    });
                }
            }

            return View(wishlistItems); // Make sure you have a view for Wishlist/List
        }
    }

    // Model for toggle
    public class WishlistToggleModel
    {
        public int PropertyId { get; set; }
    }

    // Model for displaying wishlist items
    public class WishlistItem
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
    }
}

