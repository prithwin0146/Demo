namespace EmployeeApi.Models;

public class EmployeeProject
{
    public int EmployeeProjectId { get; set; }
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.Now;
    public string? Role { get; set; }
}
