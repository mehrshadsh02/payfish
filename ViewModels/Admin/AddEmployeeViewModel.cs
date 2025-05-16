using System.ComponentModel.DataAnnotations;
using payfish.Models;
using System.Collections.Generic;

namespace payfish.ViewModels.Admin
{
    public class AddEmployeeViewModel
    {
        [Required(ErrorMessage = "کد پرسنلی الزامی است")]
        public string Code { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "نام کامل الزامی است")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "پست سازمانی الزامی است")]
        public string Position { get; set; }

        [Required(ErrorMessage = "نقش الزامی است")]
        public int RoleId { get; set; }  // 👈 جایگزین Role قبلی

        public List<Role> Roles { get; set; } = new(); // 👈 برای Dropdown
    }
}
