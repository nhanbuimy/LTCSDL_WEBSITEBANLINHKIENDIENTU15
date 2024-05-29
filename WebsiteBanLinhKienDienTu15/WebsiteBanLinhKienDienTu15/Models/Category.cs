using System.ComponentModel.DataAnnotations;

namespace WebsiteBanLinhKienDienTu15.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string CategoryName { get; set; }
    }
}
