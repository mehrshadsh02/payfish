using System;

namespace payfish.Models
{
    public class Paystub
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string FilePath { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public Employee Employee { get; set; }
    }
}