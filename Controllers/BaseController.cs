using Microsoft.AspNetCore.Mvc;

public class BaseController : Controller
{
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
}
