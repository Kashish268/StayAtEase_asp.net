using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;

public class PropertyReviewsController : Controller
{
    private List<PropertyReview> reviews = new List<PropertyReview>
    {
        new PropertyReview { Id = 1, Name = "John Smith", Date = "Feb 15, 2024", Rating = 5, Text = "Great property with excellent amenities.", ImageUrl = "/images/profile.png" },
        new PropertyReview { Id = 2, Name = "Emma Wilson", Date = "Feb 14, 2024", Rating = 5, Text = "Absolutely love living here!", ImageUrl = "/images/profile.png" },
        new PropertyReview { Id = 3, Name = "Michael Brown", Date = "Feb 13, 2024", Rating = 4, Text = "Good value for money.", ImageUrl = "/images/profile.png" }
    };

    public IActionResult Index(string searchTerm, int page = 1, int pageSize = 10)
    {
        var filteredReviews = string.IsNullOrEmpty(searchTerm)
            ? reviews
            : reviews.Where(r => r.Name.ToLower().Contains(searchTerm.ToLower())).ToList();

        int totalReviews = filteredReviews.Count();
        var paginatedReviews = filteredReviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.TotalReviews = totalReviews;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.SearchTerm = searchTerm;

        return View(paginatedReviews);
    }
}
