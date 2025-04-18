namespace payfish.Models
{
    public class LeaveDate
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; } // اختیاری (مثلاً دلیل مرخصی)
    }
}