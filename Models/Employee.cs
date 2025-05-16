    namespace payfish.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Code { get; set; } // کد ملی یا پرسنلی
        public string Password { get; set; } // رمز عبور
        public string FullName { get; set; }
        public DateTime HireDate { get; set; } // مقدار پیش‌فرض // تاریخ استخدام
        public string Position { get; set; }   // پست سازمانی
        public int RoleId { get; set; } = 1; // پیش‌فرض: Employee
        public Role Role { get; set; }       // ارتباط ناوبری

        public List<LeaveDate> LeaveDates { get; set; } = new();
        public List<Paystub> Paystubs { get; set; } // فیش‌های حقوقی
    }
}
