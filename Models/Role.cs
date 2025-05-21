namespace payfish.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? DisplayName { get; set; } // نام فارسی نقش

        public List<Employee> Employees { get; set; }
    }
}
