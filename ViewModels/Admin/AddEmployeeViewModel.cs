using System.ComponentModel.DataAnnotations;

namespace payfish.ViewModels.Admin
{
    public class AddEmployeeViewModel
    {
        [Required(ErrorMessage = "کد پرسنلی را وارد کنید")]
        public string Code { get; set; }

        [Required(ErrorMessage = "رمز عبور را وارد کنید")]
        [MinLength(6, ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد")]
        public string Password { get; set; }

        [Required(ErrorMessage = "نام کامل را وارد کنید")]
        public string FullName { get; set; }
    }
}
