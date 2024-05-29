using Microsoft.AspNetCore.Identity;

namespace WebsiteBanLinhKienDienTu15.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }
    }
}
