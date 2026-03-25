using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static ApplicationDbContext;

namespace E_Commerce_System.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string CartKey = "LaJessCart";

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }


        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartKey);
            return string.IsNullOrEmpty(json)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));
            HttpContext.Session.SetString("CartCount",
                cart.Sum(c => c.Quantity).ToString());
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(c => c.SubTotal);
            ViewBag.Shipping = cart.Sum(c => c.SubTotal) >= 2000 ? 0 : 150;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity, string selectedSize)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null || product.Stock <= 0)
            {
                TempData["CartError"] = "Product not available.";
                return RedirectToAction("Detail", "Product", new { id = productId });
            }

            quantity = Math.Max(1, Math.Min(quantity, product.Stock));

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c =>
                c.ProductId == productId &&
                c.SelectedSize == selectedSize);

            if (existing != null)
            {
                existing.Quantity = Math.Min(existing.Quantity + quantity, product.Stock);
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity,
                    SelectedSize = selectedSize,
                    CategoryName = product.Category?.Name ?? string.Empty
                });
            }

            SaveCart(cart);
            TempData["Success"] = $"\"{product.Name}\" added to your basket!";
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int productId, string selectedSize, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c =>
                c.ProductId == productId &&
                c.SelectedSize == selectedSize);

            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int productId, string selectedSize)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c =>
                c.ProductId == productId &&
                c.SelectedSize == selectedSize);

            if (item != null)
            {
                cart.Remove(item);
                TempData["Success"] = $"\"{item.ProductName}\" removed from basket.";
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            TempData["Success"] = "Your basket has been cleared.";
            return RedirectToAction("Index");
        }

        public IActionResult Count()
        {
            var cart = GetCart();
            var count = cart.Sum(c => c.Quantity);
            return Json(new { count });
        }
    }
}
