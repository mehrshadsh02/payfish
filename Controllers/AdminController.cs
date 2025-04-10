using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using payfish.Data;
using payfish.Models;
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Username == username && a.PasswordHash == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("AdminId", admin.Id.ToString());
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "نام کاربری یا رمز عبور اشتباه است.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
                return RedirectToAction("Login");

            return View(); // صفحه پنل ادمین
        }
    }
}
