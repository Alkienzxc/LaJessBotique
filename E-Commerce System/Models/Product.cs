using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Stock must be 0 or more.")]
        [Display(Name = "Stock Quantity")]
        public int Stock { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Available Sizes (comma-separated)")]
        public string Sizes { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Available Colors (comma-separated)")]
        public string Colors { get; set; } = string.Empty;

        [Display(Name = "Featured on Homepage")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Mark as New Arrival")]
        public bool IsNewArrival { get; set; } = false;

        [Display(Name = "Date Added")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Navigation
        public Category? Category { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed helpers (not mapped to DB)
        [NotMapped]
        public double AverageRating =>
            Reviews.Any() ? Math.Round(Reviews.Average(r => r.Rating), 1) : 0;

        [NotMapped]
        public bool IsInStock => Stock > 0;

        [NotMapped]
        public bool IsLowStock => Stock > 0 && Stock <= 5;
    }
}
