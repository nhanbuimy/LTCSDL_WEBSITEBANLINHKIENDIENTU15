using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteBanLinhKienDienTu15.Models
{
	public class OrderDetails
	{
		[Required]
		[Display(Name = "Order")]
		public int OrderID { get; set; }
		[ForeignKey("OrderID")]
		public Order Order { get; set; }

		[Required]
		[Display(Name = "Product")]
		public int ProductID { get; set; }
		[ForeignKey("ProductID")]
		public Product Product { get; set; }

		[Display(Name = "Quantity")]
		public int Quantity { get; set; }
		[Display(Name = "Unit Price")]
		public decimal UnitPrice { get; set; }
	}
}
