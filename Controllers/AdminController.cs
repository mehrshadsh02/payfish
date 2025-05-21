using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payfish.Data;
using payfish.Models;
using payfish.ViewModels.Admin;

namespace payfish.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly PayfishDbContext _context;

        public AdminController(PayfishDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var recentEmployees = _context.Employees
                .Include(e => e.Role)
                .OrderByDescending(e => e.HireDate)
                .Take(5)
                .ToList();

            return View("AdminDashboard", recentEmployees);
        }

        public async Task<IActionResult> EmployeeList()
        {
            var currentUserCode = User.Identity.Name;

            var employees = await _context.Employees
                .Where(e => e.Code != currentUserCode) // ❗️ کاربر خودش نمایش داده نشه
                .Select(e => new EmployeeListViewModel
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Code = e.Code,
                    HireDate = e.HireDate,
                    Position = e.Position,
                    RoleName = e.Role.Name
                })
                .ToListAsync();

            return View(employees);
        }


        [HttpGet]
        public IActionResult AddEmployee()
        {
            var model = new AddEmployeeViewModel
            {
                Roles = _context.Roles.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = _context.Roles.ToList(); // ❗برای بازگشت به فرم
                return View(model);
            }

            var employee = new Employee
            {
                Code = model.Code,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Position = model.Position,
                HireDate = DateTime.Now,
                RoleId = model.RoleId  // 👈 این خط مهمه
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction("EmployeeList");
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _context.Employees
                .Include(e => e.LeaveDates)
                .FirstOrDefault(e => e.Id == id);

            if (employee == null) return NotFound();

            var model = new EditEmployeeViewModel
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Code = employee.Code,
                Position = employee.Position,
                HireDate = employee.HireDate,
                LeaveDays = employee.LeaveDates?.ToList() ?? new List<LeaveDate>(),
                RoleId = employee.RoleId,
                Roles = _context.Roles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(EditEmployeeViewModel model, string HireDate)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = _context.Roles.ToList(); // ❗برای بازگشت به فرم
                return View(model);
            }

            var employee = _context.Employees.Include(e => e.LeaveDates).FirstOrDefault(e => e.Id == model.Id);
            if (employee == null) return NotFound();

            // تبدیل تاریخ
            if (!string.IsNullOrWhiteSpace(HireDate))
            {
                try
                {
                    var parts = HireDate.Split('/');
                    var pc = new System.Globalization.PersianCalendar();
                    employee.HireDate = pc.ToDateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), 0, 0, 0, 0);
                }
                catch
                {
                    ModelState.AddModelError("HireDate", "فرمت تاریخ نامعتبر است.");
                    model.Roles = _context.Roles.ToList();
                    return View(model);
                }
            }

            //employee.FullName = model.FullName;
            var names = model.FullName?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            employee.FirstName = names?.FirstOrDefault() ?? "";
            employee.LastName = names?.Length > 1 ? names[1] : "";
           
            employee.Code = model.Code;
            employee.Position = model.Position;
            employee.RoleId = model.RoleId;

            _context.LeaveDates.RemoveRange(employee.LeaveDates);
            employee.LeaveDates = model.LeaveDays;

            await _context.SaveChangesAsync();
            return RedirectToAction("EmployeeList");
        }


        [HttpPost]
        public async Task<IActionResult> UploadPaystub(int employeeId, int Month, int Year, IFormFile paystubFile)
        {
            if (paystubFile == null || paystubFile.Length == 0)
                return RedirectToAction("EditEmployee", new { id = employeeId });

            var fileName = $"{employeeId}_{Year}_{Month}.pdf";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "PrivatePdfs");
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
