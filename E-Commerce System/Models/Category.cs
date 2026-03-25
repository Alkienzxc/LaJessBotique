using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Icon Class (Bootstrap Icons)")]
        public string IconClass { get; set; } = "bi bi-tag";

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
