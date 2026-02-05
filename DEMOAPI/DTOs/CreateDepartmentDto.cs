using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeApi.DTOs;

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
    [JsonPropertyName("departmentName")]
    public string DepartmentName { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("managerId")]
    public int? ManagerId { get; set; }
}
