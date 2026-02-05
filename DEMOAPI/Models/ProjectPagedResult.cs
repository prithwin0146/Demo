namespace EmployeeApi.Models
{
    public class ProjectPagedResult
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AssignedEmployees { get; set; }
        public int TotalCount { get; set; }
    }
}
