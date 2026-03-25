using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_System.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(
            string name,
            string email,
            string subject,
            string message)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(message))
            {
                TempData["ContactError"] = "Please fill in all fields.";
                return RedirectToAction("Index");
            }


            TempData["Success"] = $"Thank you, {name.Trim()}! We'll get back to you within 24 hours.";
            return RedirectToAction("Index");
        }
    }
}
