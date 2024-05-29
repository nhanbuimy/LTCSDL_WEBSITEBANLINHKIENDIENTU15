using System.ComponentModel.DataAnnotations;

namespace WebsiteBanLinhKienDienTu15.Areas.Admin.Models
{
	public class RoleUserAssignVm
	{
		[Required]
		[Display(Name = "User")]
		public string UserID { get; set; }
		[Required]
		[Display(Name = "Role")]
		public string RoleID { get; set; }
	}
}
