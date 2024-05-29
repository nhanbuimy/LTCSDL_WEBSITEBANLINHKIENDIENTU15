using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteBanLinhKienDienTu15.Data;
using WebsiteBanLinhKienDienTu15.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebsiteBanLinhKienDienTu15.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class ProductController : Controller
	{
		private LinhKienDbContext _db;
		private IHostingEnvironment _he;

		public ProductController(LinhKienDbContext db, IHostingEnvironment he)
		{
			_db = db;
			_he = he;
		}
		public IActionResult Index()
		{
			return View(_db.Product.Include(c=>c.Category).Include(f=>f.SpecialTag).ToList());
		}

		// Post Index action method
		[HttpPost]
		[ValidateAntiForgeryToken]
		
		public IActionResult Index(decimal? lowAmount, decimal? highAmount, string? productName)
		{
			var query = _db.Product.Include(c => c.Category).Include(f => f.SpecialTag).AsQueryable();

			if (productName != null)
			{
				productName = productName.Trim();
				query = query.Where(p => p.ProductName.ToLower().Contains(productName.ToLower()));
			}

			if (lowAmount != null)
			{
				query = query.Where(p => p.Price >= lowAmount);
			}

			if (highAmount != null)
			{
				query = query.Where(p => p.Price <= highAmount);
			}

			var products = query.ToList();

			return View(products);
		}

		// Get Create action method

		public IActionResult Create()
		{
			ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
			ViewData["tagID"] = new SelectList(_db.SpecialTag.ToList(), "SpecialTagID", "SpecialTagName");
			return View();
		}

		// Post Create method

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create (Product product, IFormFile image)
		{
			ModelState.Clear(); // Khi ModelState.IsValid bi invalid nhung them ! vao dau thi chay binh thuong => Them dong nay
			if (ModelState.IsValid)
            {
				var searchProduct = _db.Product.FirstOrDefault(c => c.ProductName == product.ProductName);
				if (searchProduct != null) // Neu san pham da ton tai thi thong bao ra va lam moi combobox
				{
					ViewBag.message = "This product already exists";
					ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
					ViewData["tagID"] = new SelectList(_db.SpecialTag.ToList(), "SpecialTagID", "SpecialTagName");
					return View();
				}

				if (image != null)
				{
					var name = Path.Combine(_he.WebRootPath+"/Images", Path.GetFileName(image.FileName));
					await image.CopyToAsync(new FileStream(name, FileMode.Create));
					product.Image = "Images/" + image.FileName;
				}

				if (image == null) 
				{
					product.Image = "Images/noimage.png";
				}
                _db.Product.Add(product);
				await _db.SaveChangesAsync();
				TempData["create"] = "Product has been created";
				return RedirectToAction(nameof(Index));
            }

			return View(product);
        }

		// Get Edit action Method

		public ActionResult Edit(int? id)
		{
			ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
			ViewData["tagID"] = new SelectList(_db.SpecialTag.ToList(), "SpecialTagID", "SpecialTagName");
			if (id == null)
			{
				return NotFound();
			}

			var product = _db.Product.Include(c => c.Category).Include(f => f.SpecialTag)
				.FirstOrDefault(c => c.ProductID == id);
			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}

		// Post Edit action method
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Product product, IFormFile image)
		{
			var findProd = _db.Product.Where(c => c.ProductID == product.ProductID).AsNoTracking().FirstOrDefault();

			ModelState.Clear(); 
			if (ModelState.IsValid)
			{
				if (image != null)
				{
					var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
					await image.CopyToAsync(new FileStream(name, FileMode.Create));
					product.Image = "Images/" + image.FileName;
				}

				if (image == null)
				{
					product.Image = findProd.Image;
				}
				_db.Update(product);
				await _db.SaveChangesAsync();
				TempData["edit"] = "Product has been updated";
				return RedirectToAction(nameof(Index));
			}

			return View(product);
		}

		// Get Details action method

		public ActionResult Details(int? id)
		{
			ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
			ViewData["tagID"] = new SelectList(_db.SpecialTag.ToList(), "SpecialTagID", "SpecialTagName");
			if (id == null)
			{
				return NotFound();
			}

			var product = _db.Product.Include(c => c.Category).Include(f => f.SpecialTag)
				.FirstOrDefault(c => c.ProductID == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// Get Delete action method

		public ActionResult Delete(int? id)
		{
			ViewData["categoryID"] = new SelectList(_db.Category.ToList(), "CategoryID", "CategoryName");
			ViewData["tagID"] = new SelectList(_db.SpecialTag.ToList(), "SpecialTagID", "SpecialTagName");
			if (id == null)
			{
				return NotFound();
			}

			var product = _db.Product.Include(c => c.Category).Include(f => f.SpecialTag)
				.Where(c => c.ProductID == id).FirstOrDefault();
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// Post Delete action method

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirm(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = _db.Product.FirstOrDefault(c => c.ProductID == id);
			if (product == null)
			{
				return NotFound();
			}

			_db.Product.Remove(product);
			await _db.SaveChangesAsync();
			TempData["delete"] = "Product has been deleted";
			return RedirectToAction(nameof(Index));
		}
	}
}
