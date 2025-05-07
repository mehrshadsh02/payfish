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

        // اکشن لاگین
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
        [HttpPost]
        public async Task<IActionResult> EditEmployee(EditEmployeeViewModel model, string HireDate)
        {
            if (!ModelState.IsValid)
                return View(model);

            var employee = _context.Employees.Include(e => e.LeaveDates).FirstOrDefault(e => e.Id == model.Id);
            if (employee == null)
                return NotFound();

            // 🛠 تبدیل تاریخ شمسی به میلادی
            if (!string.IsNullOrWhiteSpace(HireDate))
            {
                try
                {
                    var persian = new System.Globalization.PersianCalendar();
                    var parts = HireDate.Split('/');
                    var year = int.Parse(parts[0]);
                    var month = int.Parse(parts[1]);
                    var day = int.Parse(parts[2]);
                    employee.HireDate = persian.ToDateTime(year, month, day, 0, 0, 0, 0);
                }
                catch
                {
                    ModelState.AddModelError("HireDate", "فرمت تاریخ نامعتبر است.");
                    return View(model);
                }
            }

            // سایر فیلدها
            employee.FullName = model.FullName;
            employee.Code = model.Code;
            employee.Position = model.Position;

            // حذف و ثبت مجدد مرخصی‌ها (در صورت نیاز)
            _context.LeaveDates.RemoveRange(employee.LeaveDates);
            employee.LeaveDates = model.LeaveDays;

            await _context.SaveChangesAsync();
            return RedirectToAction("EmployeeList");
        }

      
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UploadPaystub(int employeeId, int Month, int Year, IFormFile paystubFile)
        {
            if (paystubFile == null || paystubFile.Length == 0)
                return RedirectToAction("EditEmployee", new { id = employeeId });

            var fileName = $"{employeeId}_{Year}_{Month}.pdf";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "paystubs");
            Directory.CreateDirectory(uploadPath);
            var fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await paystubFile.CopyToAsync(stream);
            }

            var existing = await _context.Paystubs
                .FirstOrDefaultAsync(p => p.EmployeeId == employeeId && p.Year == Year && p.Month == Month);

            if (existing != null)
            {
                // به‌روزرسانی
                existing.FileName = fileName;
                existing.UploadDate = DateTime.Now;
            }
            else
            {
                _context.Paystubs.Add(new Paystub
                {
                    EmployeeId = employeeId,
                    Year = Year,
                    Month = Month,
                    FileName = fileName,
                    UploadDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("EditEmployee", new { id = employeeId });
        }


    }
}
