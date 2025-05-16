using Microsoft.EntityFrameworkCore;
using payfish.Models;

namespace payfish.Data
{
    public class PayfishDbContext : DbContext
    {
        public PayfishDbContext(DbContextOptions<PayfishDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Paystub> Paystubs { get; set; }
        public DbSet<LeaveDate> LeaveDates { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Employee" },
                new Role { Id = 2, Name = "Admin" }
            );

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}