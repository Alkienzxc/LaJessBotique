using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Supplier / Company Name")]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        [Display(Name = "Business Address")]
        public string Address { get; set; } = string.Empty;

        [MaxLength(300)]
        [Display(Name = "Product Types Supplied")]
        public string ProductTypes { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Notes / Remarks")]
        public string Notes { get; set; } = string.Empty;

        [Display(Name = "Active Supplier")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Date Added")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
