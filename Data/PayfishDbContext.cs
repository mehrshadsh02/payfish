    using Microsoft.EntityFrameworkCore;
    using payfish.Models;
    using System.Collections.Generic;

    namespace payfish.Data
    {
        public class PayfishDbContext : DbContext
        {
            public PayfishDbContext(DbContextOptions<PayfishDbContext> options) : base(options) { }

            public DbSet<Employee> Employees { get; set; }
            public DbSet<Paystub> Paystubs { get; set; }
            public DbSet<Admin> Admins { get; set; }
            // Data/PayfishDbContext.cs
            public DbSet<LeaveDate> LeaveDates { get; set; }
         



    }
}
