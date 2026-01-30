namespace EmployeeApi.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string JobRole { get; set; } = null!;
    public string SystemRole { get; set; } = null!;
}
