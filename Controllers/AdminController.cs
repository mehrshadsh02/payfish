using payfish.Data;
using payfish.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using payfish.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

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
                    Position = e.Position,     // تستی فعلاً
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
                Password = model.Password,
                FullName = model.FullName,
                Position = model.Position, // 👈 این خط ضروریه
                HireDate = DateTime.Now    // می‌تونیم HireDate رو هم مقدار بدیم
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("EmployeeList");
        }

        //متد های ادیت کردن کارمند ها
        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _context.Employees
                .Include(e => e.LeaveDates)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null) return NotFound();

            var viewModel = new EditEmployeeViewModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Code = employee.Code,
                Position = employee.Position,
                HireDate = employee.HireDate,
                LeaveDays = employee.LeaveDates?.ToList() ?? new List<LeaveDate>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EditEmployeeViewModel model)
        {
            if (model.HireDate <= DateTime.MinValue)
            {
                ModelState.AddModelError("HireDate", "تاریخ استخدام معتبر نیست.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var employee = _context.Employees.Include(e => e.LeaveDates).FirstOrDefault(e => e.Id == model.Id);
            if (employee == null)
                return NotFound();

            // بروزرسانی اطلاعات کارمند
            employee.FullName = model.FullName;
            employee.Code = model.Code;
            employee.Position = model.Position;
            employee.HireDate = model.HireDate;

            // حذف تاریخ‌های قبلی و افزودن جدید
            _context.LeaveDates.RemoveRange(employee.LeaveDates);

            employee.LeaveDates = model.LeaveDays
                .Where(l => l.Date > DateTime.MinValue)
                .Select(l => new LeaveDate
                {
                    Id = l.Id,
                    Date = l.Date,
                    Description = l.Description,
                    EmployeeId = model.Id
                }).ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction("EmployeeList");
        }
    }
}
