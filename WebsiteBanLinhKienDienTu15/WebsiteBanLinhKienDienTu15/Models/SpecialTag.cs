using System.ComponentModel.DataAnnotations;

namespace WebsiteBanLinhKienDienTu15.Models
{
	public class SpecialTag
	{
		public int SpecialTagID { get; set; }
		[Required]
		[Display(Name = "SpecialTags")]
		public string SpecialTagName { get; set; }
	}
}
