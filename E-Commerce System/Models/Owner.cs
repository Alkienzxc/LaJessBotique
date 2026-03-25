using System.ComponentModel.DataAnnotations;

namespace E_Commerce_System.Models
{
    public class Owner
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        [Display(Name = "Role / Title")]
        public string Role { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        [Display(Name = "Biography")]
        public string Bio { get; set; } = string.Empty;

        [Display(Name = "Profile Photo URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Facebook URL")]
        public string Facebook { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Instagram URL")]
        public string Instagram { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "LinkedIn URL")]
        public string LinkedIn { get; set; } = string.Empty;
    }
}
