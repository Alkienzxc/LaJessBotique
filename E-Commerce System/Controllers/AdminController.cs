using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static ApplicationDbContext;

namespace E_Commerce_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalProducts = await _db.Products.CountAsync();
            ViewBag.LowStockCount = await _db.Products.CountAsync(p => p.Stock > 0 && p.Stock <= 5);
            ViewBag.OutOfStockCount = await _db.Products.CountAsync(p => p.Stock == 0);
            ViewBag.TotalOrders = await _db.Orders.CountAsync();
            ViewBag.PendingOrders = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            ViewBag.TotalRevenue = await _db.Orders.Where(o => o.Status != OrderStatus.Cancelled).SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.TotalReviews = await _db.Reviews.CountAsync();
            ViewBag.TotalSuppliers = await _db.Suppliers.CountAsync(s => s.IsActive);

            ViewBag.RecentOrders = await _db.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            ViewBag.LowStockProducts = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.Stock <= 5)
                .OrderBy(p => p.Stock)
                .Take(6)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> Inventory(string? search, int? categoryId)
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            var query = _db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Category!.Name.ToLower().Contains(term));
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var products = await query.OrderBy(p => p.Name).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                return View(product);
            }

            product.CreatedAt = DateTime.Now;
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"\"{product.Name}\" added to inventory.";
            return RedirectToAction("Inventory");
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _db.Categories.ToListAsync();
                return View(product);
            }

            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"\"{product.Name}\" updated successfully.";
            return RedirectToAction("Inventory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"\"{product.Name}\" deleted.";
            }
            return RedirectToAction("Inventory");
        }

        public async Task<IActionResult> Orders(string? status)
        {
            ViewBag.StatusFilter = status;

            var query = _db.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<OrderStatus>(status, out var parsed))
                query = query.Where(o => o.Status == parsed);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Order {order.OrderNumber} status updated to {status}.";
            }
            return RedirectToAction("OrderDetail", new { id = orderId });
        }

        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await _db.Suppliers
                .OrderByDescending(s => s.IsActive)
                .ThenBy(s => s.Name)
                .ToListAsync();
            return View(suppliers);
        }

        public IActionResult CreateSupplier()
        {
            return View(new Supplier());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            supplier.CreatedAt = DateTime.Now;
            _db.Suppliers.Add(supplier);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"\"{supplier.Name}\" added to suppliers.";
            return RedirectToAction("Suppliers");
        }

        public async Task<IActionResult> EditSupplier(int id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return View(supplier);

            _db.Suppliers.Update(supplier);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"\"{supplier.Name}\" updated.";
            return RedirectToAction("Suppliers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _db.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _db.Suppliers.Remove(supplier);
                await _db.SaveChangesAsync();
                TempData["Success"] = $"\"{supplier.Name}\" deleted.";
            }
            return RedirectToAction("Suppliers");
        }

        public async Task<IActionResult> Reviews()
        {
            var reviews = await _db.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review != null)
            {
                _db.Reviews.Remove(review);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Review deleted.";
            }
            return RedirectToAction("Reviews");
        }
    }
}
