using System;

namespace payfish.Models
{
    public class Paystub
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        public int Year { get; set; }       // مثل 1403
        public int Month { get; set; }      // مثل 2

        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }

        public Employee Employee { get; set; }

    }
}