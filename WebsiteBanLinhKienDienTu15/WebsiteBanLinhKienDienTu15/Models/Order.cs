using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanLinhKienDienTu15.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "User ID")]
        [ForeignKey("Id")]
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
