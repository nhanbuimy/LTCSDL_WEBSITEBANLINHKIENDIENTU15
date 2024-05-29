using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanLinhKienDienTu15.Data;
using WebsiteBanLinhKienDienTu15.Models;

namespace WebsiteBanLinhKienDienTu15.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class SpecialTagController : Controller
	{
        private LinhKienDbContext _db;
		public SpecialTagController(LinhKienDbContext db)
		{
			_db = db;
		}
		public IActionResult Index()
		{
			return View(_db.SpecialTag.ToList());
		}

		// Get Create action Method

		public ActionResult Create()
        {
            return View();
        }

        // POST Create action Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.SpecialTag.Add(specialTag);
                await _db.SaveChangesAsync();
				TempData["create"] = "Tag has been created";
				return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }

		// Get Edit action Method

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var specialTag = _db.SpecialTag.Find(id);
			if (specialTag == null)
			{
				return NotFound();
			}
			return View(specialTag);
		}

		// POST Edit action Method

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(SpecialTag specialTag)
		{
			if (ModelState.IsValid)
			{
				_db.Update(specialTag);
				await _db.SaveChangesAsync();
				TempData["edit"] = "Tag has been updated";
				return RedirectToAction(nameof(Index));
			}

			return View(specialTag);
		}

		// Get Details action Method

		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var specialTag = _db.SpecialTag.Find(id);
			if (specialTag == null)
			{
				return NotFound();
			}
			return View(specialTag);
		}

		// POST Details action Method

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Details(SpecialTag specialTag)
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

			var specialTag = _db.SpecialTag.Find(id);
			if (specialTag == null)
			{
				return NotFound();
			}

			if (TempData.ContainsKey("deleteSpecialTagError"))
			{
				// Pass the delete error message to the view using ViewBag
				ViewBag.DeleteSpecialTagError = TempData["deleteSpecialTagError"];
			}

			return View(specialTag);
		}

		// POST Delete action Method

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int? id, SpecialTag specialTags)
		{
			if (id == null)
			{
				return NotFound();
			}

			if (id != specialTags.SpecialTagID)
			{
				return NotFound();
			}

			var specialTag = _db.SpecialTag.Find(id);
			if (specialTag == null)
			{
				return NotFound();
			}

			var productsInSpecialTag = await _db.Product.AnyAsync(p => p.SpecialTagID == id);
			if (productsInSpecialTag)
			{
				// If there are products in this category, display a message
				TempData["deleteSpecialTagError"] = "Cannot delete tag. There are products associated with this tag.";

				return RedirectToAction(nameof(Delete), new { id });
			}

			if (ModelState.IsValid)
			{
				_db.Remove(specialTag);
				await _db.SaveChangesAsync();
				TempData["delete"] = "Tag has been deleted";
				return RedirectToAction(nameof(Index));
			}

			return View(specialTags);
		}
	}
}
