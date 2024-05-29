using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteBanLinhKienDienTu15.Data;
using WebsiteBanLinhKienDienTu15.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<LinhKienDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LinhKienDbContext>();

builder.Services.AddScoped<UserManager<ApplicationUser>>();

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    // You can change the password requirement here
    options.Password.RequireUppercase = false;
});

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
    );
});
app.MapRazorPages();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	try
	{
		var context = services.GetRequiredService<LinhKienDbContext>();
		context.Database.Migrate(); // Apply migrations

		// Seed data if necessary
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

		if (!roleManager.Roles.Any())
		{
			var roles = new[] { "Admin", "User" };
			foreach (var role in roles)
			{
				await roleManager.CreateAsync(new IdentityRole(role));
			}
		}

		if (!userManager.Users.Any(u => u.UserName == "nam_thinh_nhan"))
		{
			var adminUser = new ApplicationUser
			{
				UserName = "nam_thinh_nhan",
				Email = "admin@admin.com",
				EmailConfirmed = false
			};
			await userManager.CreateAsync(adminUser, "ad@123");
			await userManager.AddToRoleAsync(adminUser, "Admin");
		}
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while migrating or seeding the database.");
	}
}

app.Run();
