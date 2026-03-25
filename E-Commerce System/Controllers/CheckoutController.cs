using E_Commerce_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static ApplicationDbContext;

namespace E_Commerce_System.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string CartKey = "LaJessCart";

        public CheckoutController(ApplicationDbContext db)
        {
            _db = db;
        }

        private List<CartItem> GetCart() =>
            JsonSerializer.Deserialize<List<CartItem>>(
                HttpContext.Session.GetString(CartKey) ?? "[]")
            ?? new List<CartItem>();

        private void ClearCart()
        {
            HttpContext.Session.Remove(CartKey);
            HttpContext.Session.SetString("CartCount", "0");
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            var subtotal = cart.Sum(c => c.SubTotal);
            ViewBag.Cart = cart;
            ViewBag.Subtotal = subtotal;
            ViewBag.Shipping = subtotal >= 2000 ? 0 : 150;
            ViewBag.Total = subtotal + (subtotal >= 2000 ? 0 : 150);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(
            string customerName,
            string customerEmail,
            string customerPhone,
            string shippingAddress,
            string? notes)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index", "Cart");

            foreach (var item in cart)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    TempData["CheckoutError"] =
                        $"Sorry, \"{item.ProductName}\" no longer has enough stock. Please update your basket.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            var subtotal = cart.Sum(c => c.SubTotal);
            var shipping = subtotal >= 2000 ? 0 : 150;

            var orderCount = await _db.Orders.CountAsync();
            var orderNumber = $"LJ-{DateTime.Now.Year}-{(orderCount + 1):D4}";

            var order = new Order
            {
                OrderNumber = orderNumber,
                CustomerName = customerName.Trim(),
                CustomerEmail = customerEmail.Trim(),
                CustomerPhone = customerPhone.Trim(),
                ShippingAddress = shippingAddress.Trim(),
                TotalAmount = subtotal + shipping,
                Status = OrderStatus.Confirmed,
                OrderDate = DateTime.Now,
                Notes = notes?.Trim()
            };

            foreach (var item in cart)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    SelectedSize = item.SelectedSize
                });

                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null)
                    product.Stock -= item.Quantity;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            ClearCart();

            TempData["Success"] = $"Order {orderNumber} placed successfully!";
            return RedirectToAction("Confirmation", new { orderNumber });
        }

        public async Task<IActionResult> Confirmation(string orderNumber)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return RedirectToAction("Index", "Home");

            return View(order);
        }

        public async Task<IActionResult> Track(string? orderNumber)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                return View();

            var order = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o =>
                    o.OrderNumber.ToLower() == orderNumber.Trim().ToLower());

            if (order == null)
                ViewBag.NotFound = true;
            else
                ViewBag.Order = order;

            ViewBag.SearchedNumber = orderNumber.Trim().ToUpper();
            return View();
        }
    }
}
