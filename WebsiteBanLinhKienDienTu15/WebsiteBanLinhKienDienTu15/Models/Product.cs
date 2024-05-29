using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebsiteBanLinhKienDienTu15.Models
{
    public class Product
    {
        [Required]
        public int ProductID { get; set; }

        [Required]
        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }

        [Display(Name = "Product Color")]
        public string? ProductColor { get; set; }

        [Required]
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category Category { get; set; }

        [Display(Name = "Special Tag")]
        [Required]
        public int SpecialTagID { get; set; }
        [ForeignKey("SpecialTagID")]
        public SpecialTag SpecialTag { get; set; }
    }
}
