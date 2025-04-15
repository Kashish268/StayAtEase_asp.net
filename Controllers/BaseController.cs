using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class BaseController : Controller
{
    private readonly IConfiguration _configuration;

    public BaseController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected bool IsUserLoggedIn()
    {
        return HttpContext.Session.GetInt32("UserId") != null;
    }

    protected IActionResult RedirectToLoginIfNotLoggedIn()
    {
        if (!IsUserLoggedIn())
        {
            return RedirectToAction("Index", "Home");
        }

        return null;
    }

    //public override void OnActionExecuting(ActionExecutingContext context)
    //{
    //    base.OnActionExecuting(context);

    //    var userId = HttpContext.Session.GetInt32("UserId");

    //    string profileImagePath = ""; // Default

    //    if (userId != null)
    //    {
    //        string connectionString = _configuration.GetConnectionString("DefaultConnection");

    //        using (SqlConnection connection = new SqlConnection(connectionString))
    //        {
    //            string query = "SELECT profile_pic FROM Users WHERE user_id = @UserId";
    //            SqlCommand command = new SqlCommand(query, connection);
    //            command.Parameters.AddWithValue("@UserId", userId);

    //            connection.Open();
    //            var result = command.ExecuteScalar();
    //            if (result != null && result != DBNull.Value)
    //            {
    //                profileImagePath = result.ToString();
    //            }
    //        }
    //    }

    //    ViewBag.ProfileImage = profileImagePath;
    //}
}

