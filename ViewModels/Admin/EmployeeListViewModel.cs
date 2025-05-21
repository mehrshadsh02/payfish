namespace payfish.ViewModels.Admin
{
    public class EmployeeListViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public DateTime HireDate { get; set; }
        public string Position { get; set; } // برای آینده یا دیتابیس، اگر بخوایم اضافه کنیم
        public string Status { get; set; } // فعال، مرخصی و...
        public int RoleId { get; set; }
        public string RoleName { get; set; }



    }
}
