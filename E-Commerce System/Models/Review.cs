using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Your Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        [Display(Name = "Rating (1–5)")]
        public int Rating { get; set; }

        [Required, MaxLength(1000)]
        [Display(Name = "Your Review")]
        public string Comment { get; set; } = string.Empty;

        [Display(Name = "Submitted On")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public string StarDisplay =>
            string.Concat(Enumerable.Repeat("★", Rating)) +
            string.Concat(Enumerable.Repeat("☆", 5 - Rating));
    }
}
