using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ApplicationDbContext;

namespace E_Commerce_System.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            string? search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort)
        {
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;

            ViewBag.Categories = await _db.Categories.ToListAsync();

            ViewBag.LowestPrice = await _db.Products.MinAsync(p => (decimal?)p.Price) ?? 0;
            ViewBag.HighestPrice = await _db.Products.MaxAsync(p => (decimal?)p.Price) ?? 9999;

            var query = _db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Stock > 0)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.Category!.Name.ToLower().Contains(term));
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                "name_asc" => query.OrderBy(p => p.Name),
                _ => query.OrderByDescending(p => p.IsFeatured)
            };

            var products = await query.ToListAsync();

            ViewBag.ResultCount = products.Count;

            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews.OrderByDescending(r => r.CreatedAt))
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            ViewBag.RelatedProducts = await _db.Products
                .Include(p => p.Reviews)
                .Where(p => p.CategoryId == product.CategoryId
                         && p.Id != id
                         && p.Stock > 0)
                .Take(4)
                .ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(
            int productId,
            string customerName,
            string customerEmail,
            int rating,
            string comment)
        {
            if (string.IsNullOrWhiteSpace(customerName) ||
                string.IsNullOrWhiteSpace(customerEmail) ||
                string.IsNullOrWhiteSpace(comment) ||
                rating < 1 || rating > 5)
            {
                TempData["ReviewError"] = "Please fill in all fields and select a rating.";
                return RedirectToAction("Detail", new { id = productId });
            }

            var review = new Review
            {
                ProductId = productId,
                CustomerName = customerName.Trim(),
                CustomerEmail = customerEmail.Trim(),
                Rating = rating,
                Comment = comment.Trim(),
                CreatedAt = DateTime.Now
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Thank you for your review!";
            return RedirectToAction("Detail", new { id = productId });
        }
    }
}
