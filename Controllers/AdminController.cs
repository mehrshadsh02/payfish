using payfish.Data;
using payfish.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace payfish.Controllers
{
    public class AdminController : Controller
    {
        private readonly PayfishDbContext _context;

        public AdminController(PayfishDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View("AdminLogin"); // 👈 نام ویو رو صراحتاً مشخص کن
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);

            if (admin != null)
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "نام کاربری یا رمز عبور اشتباه است.";
            return View("AdminLogin"); // 👈 باز هم مشخص کن
        }

        public IActionResult Dashboard()
        {
            return View("AdminDashboard"); // 👈 نام جدید View

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdminLoggedIn"); // حذف سشن
            return RedirectToAction("Login"); // برگشت به صفحه ورود
        }
    }
}
