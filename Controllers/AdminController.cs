using payfish.Data;
using payfish.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using payfish.ViewModels.Admin;

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
            var recentEmployees = _context.Employees
                .OrderByDescending(e => e.HireDate)
                .Take(5)
                .ToList();

            return View("AdminDashboard", recentEmployees);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdminLoggedIn"); // حذف سشن
            return RedirectToAction("Login"); // برگشت به صفحه ورود
        }

        public IActionResult EmployeeList()
        {
            var employees = _context.Employees
                .Select(e => new EmployeeListViewModel
                {
                    Id = e.Id,
                    Code = e.Code,
                    FullName = e.FullName,
                    HireDate = DateTime.Now, // اینو از دیتابیس اضافه می‌کنیم بعداً
                    Position = "نامشخص",     // تستی فعلاً
                    Status = "فعال"          // تستی فعلاً
                }).ToList();

            return View(employees);
        }

        // GET: نمایش فرم افزودن
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }

        // POST: پردازش فرم
        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var employee = new Employee
            {
                Code = model.Code,
                Password = model.Password, // می‌تونیم بعداً رمزگذاری هم اضافه کنیم
                FullName = model.FullName
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("EmployeeList");
        }

    }
}
