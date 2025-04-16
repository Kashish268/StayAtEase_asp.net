using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class Super_AdminController : BaseController
    {
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

           
            return View();
        }
        public IActionResult Total_Admin()
        {
            ViewData["ActivePage"] = "Total_Admin";
            return View();
        }
        public IActionResult Property_Reviews()
        {

            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

           
            ViewData["ActivePage"] = "Property_Reviews";
            return View();
        }
        public IActionResult Inquiries()
        {
            ViewData["ActivePage"] = "Inquiries";
            return View();
        }
        public IActionResult Total_Properties()
        {


            var redirect = RedirectToLoginIfNotLoggedIn();
            if (redirect != null) return redirect;

           
            ViewData["ActivePage"] = "Total_Properties";
            return View();
        }

        public IActionResult Particular_property()
        {
            ViewData["ActivePage"] = "Total_Properties";
            return View();
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


    }
}
