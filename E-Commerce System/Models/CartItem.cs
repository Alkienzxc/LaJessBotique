using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public decimal Price { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; } = 1;

        public string SelectedSize { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public decimal SubTotal => Price * Quantity;
    }
}
