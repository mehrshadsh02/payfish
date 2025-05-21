using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using payfish.Data;
using payfish.Models;
using System.Security.Claims;

namespace payfish.Controllers
{
    public class PaystubController : Controller
    {
        private readonly PayfishDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PaystubController(PayfishDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string code, string password)
        {
            var employee = _context.Employees
                 .Include(e => e.Role) // ⬅ برای دسترسی به Role.Name
                 .FirstOrDefault(e => e.Code == code && e.Password == password);

            if (employee != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                    new Claim(ClaimTypes.Name, employee.Code), // ✅ اینجا اصلاح شد
                    new Claim("FullName", employee.FullName),  // اختیاری
                    new Claim(ClaimTypes.Role, employee.Role?.Name ?? "Employee")
                };

                var identity = new ClaimsIdentity(claims, "MyCookie");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("MyCookie", principal);

                return employee.Role?.Name == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Dashboard", "Paystub");
            }
            ViewBag.Error = "کد یا رمز اشتباه است.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookie");
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Employee")]
        public IActionResult Dashboard()
        {
            var nationalCode = User.FindFirst("NationalCode")?.Value;

            var employee = _context.Employees
                .Include(e => e.Paystubs)
                .FirstOrDefault(e => e.Code == nationalCode);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult ViewPaystubSecure(int id)
        {
            var nationalCode = User.FindFirst("NationalCode")?.Value;

            var paystub = _context.Paystubs
                .Include(p => p.Employee)
                .FirstOrDefault(p => p.Id == id && p.Employee.Code == nationalCode);

            if (paystub == null || string.IsNullOrWhiteSpace(paystub.FileName))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PrivatePdfs", paystub.FileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/pdf");
        }
    }
}
