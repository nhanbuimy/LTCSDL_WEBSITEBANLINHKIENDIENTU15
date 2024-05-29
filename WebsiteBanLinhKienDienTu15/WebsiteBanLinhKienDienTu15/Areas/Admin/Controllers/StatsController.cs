using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanLinhKienDienTu15.Data;

namespace WebsiteBanLinhKienDienTu15.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StatsController : Controller
	{
        private LinhKienDbContext _db;

        public StatsController(LinhKienDbContext db)
        {
            _db = db;
        }
		public IActionResult Index()
		{
			return View();
		}

        [HttpPost]
		public async Task<IActionResult> ProductsInCategory()
		{
			var data = await _db.Product
				.GroupBy(p => p.Category.CategoryName)
				.Select(g => new
				{
                    label = g.Key,
                    value = g.Count()
				})
				.ToListAsync();

			return Json(data);
		}

        [HttpPost]
        public IActionResult ProductsInBrand()
        {
            var productsInBrand = _db.Product
                .GroupBy(p => p.SpecialTag.SpecialTagName)
                .Select(g => new
                {
                    label = g.Key,
                    value = g.Count()
                })
                .ToList();

            return Json(productsInBrand);
        }

        public IActionResult GetTotalAmountByCategory()
        {
            var result = _db.OrderDetails
                .Include(od => od.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(od => od.Product.Category.CategoryName)
                .Select(g => new
                {
                    label = g.Key,
                    value = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .ToList();

            return Json(result);
        }

        public IActionResult GetTotalAmountByBrand()
        {
            var result = _db.OrderDetails
                .Include(od => od.Product)
                .ThenInclude(p => p.SpecialTag)
                .GroupBy(od => od.Product.SpecialTag.SpecialTagName)
                .Select(g => new
                {
                    label = g.Key,
                    value = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .ToList();

            return Json(result);
        }

        [HttpPost]
        public JsonResult GetMonthlyIncome()
        {
            var result = _db.Order
                .Join(
                    _db.OrderDetails,
                    order => order.OrderID,
                    orderDetail => orderDetail.OrderID,
                    (order, orderDetail) => new { order.OrderDate, orderDetail.UnitPrice, orderDetail.Quantity }
                )
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    month = g.Key,
                    totalIncome = g.Sum(o => o.Quantity * o.UnitPrice)
                })
                .OrderBy(r => r.month)
                .ToList();

            var monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            var data = monthNames
                .Select((name, index) => new { label = name, value = result.FirstOrDefault(r => r.month == index + 1)?.totalIncome ?? 0 })
                .Where(m => !string.IsNullOrEmpty(m.label))
                .ToList();

            return Json(data);
        }

    }
}
