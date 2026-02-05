namespace EmployeeApi.Models
{
    public class EmployeePagedResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobRole { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public int TotalCount { get; set; }
    }
}
