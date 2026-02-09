namespace EmployeeApi.Models
{
    public class EmployeeProjectPagedResult
    {
        public int EmployeeProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string EmployeeJobRole { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public string? Role { get; set; }
        public int TotalCount { get; set; }
    }
}
