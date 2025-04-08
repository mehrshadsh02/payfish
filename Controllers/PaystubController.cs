using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payfish.Data;
using payfish.Models;
using System.Linq;

namespace payfish.Controllers
{
    public class PaystubController : Controller
    {
        private readonly PayfishDbContext _context;

        public PaystubController(PayfishDbContext context)
        {
            _context = context;
        }

        // نمایش فرم لاگین
        public IActionResult Login()
        {
            return View();
        }

        // پردازش فرم لاگین
        [HttpPost]
        public IActionResult Login(string code, string password)
        {
            // بررسی کارمند از دیتابیس
            var employee = _context.Employees
                .Include(e => e.Paystubs)
                .FirstOrDefault(e => e.Code == code && e.Password == password);

            if (employee != null)
            {
                // هدایت به لیست فیش‌ها بعد از لاگین موفق
                return RedirectToAction("Index", new { id = employee.Id });
            }

            ViewBag.Error = "کد یا رمز اشتباه است.";
            return View();
        }

        // نمایش لیست فیش‌های حقوقی برای هر کارمند
        public IActionResult Index(int id)
        {
            var employee = _context.Employees
                .Include(e => e.Paystubs)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return NotFound();

            ViewBag.EmployeeName = employee.FullName;
            ViewBag.EmployeeId = employee.Id;

            return View(employee.Paystubs.ToList());
        }

        // دانلود فایل فیش
        public IActionResult Download(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "paystubs", filePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            return PhysicalFile(fullPath, "application/pdf");
        }
    }
}
