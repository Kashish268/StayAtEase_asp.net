using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class AddPropertyModel
    {
        public int PropertyId { get; set; }
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

        // List to hold uploaded images
        [Required(ErrorMessage = "At least one image is required.")]
        [MaxLength(8, ErrorMessage = "You can only upload up to 8 images.")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public List<string> ExistingImages { get; set; } = new List<string>();
        // Foreign Key to associate property with a user
        [Required]
        public int UserId { get; set; }

        // Validation for file size (for each image)
        [DataType(DataType.Upload)]
        public string ImageUrl { get; set; } // First image
        public List<string> ImageFiles { get; set; } = new();

        // Add any custom validation for files (example: image size)
        public string ValidateImageFiles(IList<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > 5 * 1024 * 1024) // Limit to 5MB
                {
                    return "Each image must be less than 5MB.";
                }
                if (!file.ContentType.StartsWith("image/"))
                {
                    return "Only image files are allowed.";
                }
            }
            return string.Empty;
        }
    }
}
