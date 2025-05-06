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
                // ذخیره در سشن
                HttpContext.Session.SetInt32("EmployeeId", employee.Id);
                HttpContext.Session.SetString("EmployeeName", employee.FullName);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "کد یا رمز اشتباه است.";
            return View();
        }

        // نمایش لیست فیش‌های حقوقی برای هر کارمند
        
        public IActionResult Dashboard()
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            if (employeeId == null)
                return RedirectToAction("Login");

            var employee = _context.Employees
                .Include(e => e.Paystubs)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null)
                return NotFound();

            return View(employee); // به View با نام Dashboard.cshtml میره
        }


        // دانلود فایل فیش
        public IActionResult Download(string filePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "paystubs", filePath);
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            return PhysicalFile(fullPath, "application/pdf");
        }

        public IActionResult ViewPaystub(int id, int year, int month)
        {
            var paystub = _context.Paystubs
                .FirstOrDefault(p => p.EmployeeId == id && p.Year == year && p.Month == month);

            if (paystub == null || string.IsNullOrEmpty(paystub.FileName))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "paystubs", paystub.FileName);

            var mimeType = "application/pdf";
            return PhysicalFile(filePath, mimeType);
        }
    }
}
