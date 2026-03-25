using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static ApplicationDbContext;

namespace E_Commerce_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _db.Categories
                .Include(c => c.Products)
                .ToListAsync();

            ViewBag.FeaturedProducts = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.IsFeatured && p.Stock > 0)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            ViewBag.NewArrivals = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsNewArrival && p.Stock > 0)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToListAsync();

            ViewBag.Reviews = await _db.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .Take(4)
                .ToListAsync();

            ViewBag.TotalProducts = await _db.Products.CountAsync(p => p.Stock > 0);

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
