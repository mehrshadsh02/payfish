namespace payfish.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Code { get; set; } // کد ملی یا پرسنلی
        public string Password { get; set; } // رمز عبور
        public string FullName { get; set; }
        public List<Paystub> Paystubs { get; set; } // فیش‌های حقوقی
    }
}
