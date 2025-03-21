using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class AddPropertyModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Square footage must be greater than 0.")]
        public int SquareFootage { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Must have at least 1 bedroom.")]
        public int Bedrooms { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Must have at least 1 bathroom.")]
        public int Bathrooms { get; set; }

        [Required]
        public string PropertyType { get; set; }

        public List<string> Amenities { get; set; } = new List<string>();

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
