namespace EmployeeApi.Models;

public class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? Description { get; set; }

    public int? ManagerId { get; set; }
}
