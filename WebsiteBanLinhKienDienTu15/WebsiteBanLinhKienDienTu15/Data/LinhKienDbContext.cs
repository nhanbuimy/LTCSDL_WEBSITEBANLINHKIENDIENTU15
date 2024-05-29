using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebsiteBanLinhKienDienTu15.Models;

namespace WebsiteBanLinhKienDienTu15.Data
{
    public class LinhKienDbContext : IdentityDbContext<ApplicationUser>
    {
        public LinhKienDbContext(DbContextOptions<LinhKienDbContext> options)
            : base(options)
        {
			Database.Migrate();
		}

        public DbSet<Category> Category { get; set; }
		public DbSet<SpecialTag> SpecialTag { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Comment> Comment { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure composite primary key for OrderDetails
			modelBuilder.Entity<OrderDetails>()
				.HasKey(od => new { od.OrderID, od.ProductID });

			// Seed roles
			modelBuilder.Entity<IdentityRole>().HasData(
				new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
				new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
			);

			// Seed initial admin user
			var hasher = new PasswordHasher<ApplicationUser>();
			var adminUser = new ApplicationUser
			{
                Id = "1",
                UserName = "nam_thinh_nhan",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
			adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin@123");

			modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

			// Assign the admin user to the Admin role
			modelBuilder.Entity<IdentityUserRole<string>>().HasData(
				new IdentityUserRole<string>
				{
					UserId = "1",
					RoleId = "1" // Ensure this matches the seeded Admin role Id
				}
			);
		}
	}
}
