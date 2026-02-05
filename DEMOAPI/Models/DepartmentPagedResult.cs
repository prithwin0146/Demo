namespace EmployeeApi.Models
{
    public class DepartmentPagedResult
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public int EmployeeCount { get; set; }
        public int TotalCount { get; set; }
    }
}
