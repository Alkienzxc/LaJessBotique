using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce_System.Models
{
    public enum OrderStatus
    {
        [Display(Name = "Pending")] Pending = 0,
        [Display(Name = "Confirmed")] Confirmed = 1,
        [Display(Name = "Processing")] Processing = 2,
        [Display(Name = "Shipped")] Shipped = 3,
        [Display(Name = "Delivered")] Delivered = 4,
        [Display(Name = "Cancelled")] Cancelled = 5
    }

    public class Order
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        [Display(Name = "Order Notes")]
        public string? Notes { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed helpers (not mapped to DB)
        [NotMapped]
        public string StatusBadgeClass => Status switch
        {
            OrderStatus.Pending => "badge-pending",
            OrderStatus.Confirmed => "badge-confirmed",
            OrderStatus.Processing => "badge-processing",
            OrderStatus.Shipped => "badge-shipped",
            OrderStatus.Delivered => "badge-delivered",
            OrderStatus.Cancelled => "badge-cancelled",
            _ => "badge-pending"
        };

        [NotMapped]
        public string StatusIcon => Status switch
        {
            OrderStatus.Pending => "bi bi-clock",
            OrderStatus.Confirmed => "bi bi-check-circle",
            OrderStatus.Processing => "bi bi-gear",
            OrderStatus.Shipped => "bi bi-truck",
            OrderStatus.Delivered => "bi bi-bag-check",
            OrderStatus.Cancelled => "bi bi-x-circle",
            _ => "bi bi-clock"
        };

        [NotMapped]
        public int TrackingStep => Status switch
        {
            OrderStatus.Pending => 0,
            OrderStatus.Confirmed => 1,
            OrderStatus.Processing => 2,
            OrderStatus.Shipped => 3,
            OrderStatus.Delivered => 4,
            OrderStatus.Cancelled => -1,
            _ => 0
        };
    }

    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [MaxLength(20)]
        [Display(Name = "Size Selected")]
        public string SelectedSize { get; set; } = string.Empty;

        [Required] public int OrderId { get; set; }
        [Required] public int ProductId { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; }

        [NotMapped]
        public decimal SubTotal => UnitPrice * Quantity;
    }
}
