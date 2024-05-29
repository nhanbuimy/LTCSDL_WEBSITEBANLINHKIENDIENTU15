using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteBanLinhKienDienTu15.Areas.Admin.Models;
using WebsiteBanLinhKienDienTu15.Data;
using WebsiteBanLinhKienDienTu15.Models;

namespace WebsiteBanLinhKienDienTu15.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class RoleController : Controller
	{
		RoleManager<IdentityRole> _roleManager;
		UserManager<ApplicationUser> _userManager;
        LinhKienDbContext _db;
		
		public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, LinhKienDbContext db)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_db = db;
		}
		public IActionResult Index()
		{
			var roles = _roleManager.Roles.ToList();
			ViewBag.Roles = roles;
			return View();
		}

		// Get Create action method
		public async Task<IActionResult> Create()
		{
			return View();
		}

		// Post Create action method
		[HttpPost]
		public async Task<IActionResult> Create(string name)
		{
			IdentityRole role = new IdentityRole();
			role.Name = name;
			var isExist = await _roleManager.RoleExistsAsync(role.Name);
			if (isExist)
			{
				ViewBag.message = "This role already exists!";
				ViewBag.name = name;
				return View();
			}
			var result = await _roleManager.CreateAsync(role);
			if (result.Succeeded)
			{
				TempData["create"] = "Role has been created";
				return RedirectToAction(nameof(Index));
			}
			return View();
		}

		// Get Delete action method
		public async Task<IActionResult> Delete(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null)
			{
				return NotFound();
			}
			ViewBag.id = role.Id;
			ViewBag.name = role.Name;

            if (TempData.ContainsKey("deleteRoleError"))
            {
                // Pass the delete error message to the view using ViewBag
                ViewBag.DeleteRoleError = TempData["deleteRoleError"];
            }
            return View();
		}

		// Post Delete action method
		[HttpPost]
		[ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirm(string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role == null)
			{
				return NotFound();
			}

            if (role.Name == "Admin" || role.Name == "User")
            {
                TempData["error"] = "Cannot delete basic roles.";
                return RedirectToAction(nameof(Index));
            }

            var userInRole = await _db.UserRoles.AnyAsync(c => c.RoleId == id);
            if (userInRole)
            {
                // If there are products in this category, display a message
                TempData["deleteRoleError"] = "Cannot delete role. There are users associated with this role.";

                return RedirectToAction(nameof(Delete), new { id });
            }

            var result = await _roleManager.DeleteAsync(role);
			if (result.Succeeded)
			{
				TempData["delete"] = "Role has been deleted";
				return RedirectToAction(nameof(Index));
			}
			
			return View();
		}

		// Get Assign action method
		public async Task<IActionResult> Assign()
		{
			ViewData["UserID"] = new SelectList(_db.ApplicationUsers.Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null).ToList(), "Id", "UserName");
			ViewData["RoleID"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
			return View();
		}

		// Post Assign action method
		[HttpPost]
		public async Task<IActionResult> Assign(RoleUserAssignVm roleUser)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == roleUser.UserID);
			var isCheckRoleAssign = await _userManager.IsInRoleAsync(user, roleUser.RoleID);
			if (isCheckRoleAssign)
			{
				ViewBag.message = "This user already has this role!";
				ViewData["UserID"] = new SelectList(_db.ApplicationUsers.Where(f => f.LockoutEnd < DateTime.Now || f.LockoutEnd == null).ToList(), "Id", "UserName");
				ViewData["RoleID"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
			}
			var role = await _userManager.AddToRoleAsync(user, roleUser.RoleID);
			if (role.Succeeded)
			{
				TempData["assign"] = "User role assigned";
				return RedirectToAction(nameof(Index));
			}
			return View();
		}

		public ActionResult AssignUserRole()
		{
			var result = from u in _db.UserRoles
						 join r in _db.Roles on u.RoleId equals r.Id
						 join a in _db.ApplicationUsers on u.UserId equals a.Id
						 select new UserRoleMapping()
						 {
							 UserID = u.UserId,
							 RoleID = u.RoleId,
							 UserName = a.UserName,
							 RoleName = r.Name
						 };
			ViewBag.UserRoles = result;
			return View();
		}
	}
}
