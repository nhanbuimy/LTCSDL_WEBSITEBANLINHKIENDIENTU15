using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanLinhKienDienTu15.Data;
using WebsiteBanLinhKienDienTu15.Models;

namespace WebsiteBanLinhKienDienTu15.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private LinhKienDbContext _db;

        public CategoryController(LinhKienDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            //var data = _db.Category.ToList();
            return View(_db.Category.ToList());
        }

        // Get Create action Method

        public ActionResult Create()
        {
            return View();
        }

        // POST Create action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Category.Add(category);
                await _db.SaveChangesAsync();
				TempData["create"] = "Category has been created";
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

		// Get Edit action Method
		public ActionResult Edit(int? id)
		{
            if (id == null)
            {
                return NotFound();
            }
            var category = _db.Category.Find(id);
            if (category==null)
            {
                return NotFound();
            }
			return View(category);
		}

		// POST Edit action Method

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Category category)
		{
			if (ModelState.IsValid)
			{
				_db.Update(category);
				await _db.SaveChangesAsync();
				TempData["edit"] = "Category has been updated";
				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		// Get Details action Method
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var category = _db.Category.Find(id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

		// POST Details action Method

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Details(Category category)
		{
			return RedirectToAction(nameof(Index));
		}

		// Get Delete action Method
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var category = _db.Category.Find(id);
			if (category == null)
			{
				return NotFound();
			}

			if (TempData.ContainsKey("deleteCategoryError"))
			{
				// Pass the delete error message to the view WebsiteBanLinhKienDienTu15 ViewBag
				ViewBag.DeleteCategoryError = TempData["deleteCategoryError"];
			}

			return View(category);
		}

		// POST Delete action Method

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int? id, Category categories)
		{
			if (id == null)
			{
				return NotFound();
			}

			if (id != categories.CategoryID)
			{
				return NotFound();
			}

			var category = _db.Category.Find(id);
			if (category == null)
			{
				return NotFound();
			}

			var productsInCategory = await _db.Product.AnyAsync(p => p.CategoryID == id);
			if (productsInCategory)
			{
				// If there are products in this category, display a message
				TempData["deleteCategoryError"] = "Cannot delete category. There are products associated with this category.";
				
				return RedirectToAction(nameof(Delete), new { id });
			}

			if (ModelState.IsValid)
			{
				_db.Category. Remove(category);
				await _db.SaveChangesAsync();
				TempData["delete"] = "Category has been deleted";
				return RedirectToAction(nameof(Index));
			}

			return View(categories);
		}
	}
}
