using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ApplicationDbContext;

namespace E_Commerce_System.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _db;
            
        public AboutController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Owners = await _db.Owners.ToListAsync();

            ViewBag.Products = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.Stock > 0)
                .Take(6)
                .ToListAsync();

            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.TotalCategories = await _db.Categories.CountAsync();
            ViewBag.TotalOrders = await _db.Orders.CountAsync();
            ViewBag.TotalReviews = await _db.Reviews.CountAsync();

            return View();
        }
    }
}
