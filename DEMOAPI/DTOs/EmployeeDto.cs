using System.Text.Json.Serialization;

namespace EmployeeApi.DTOs;

public class EmployeeDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    
    [JsonPropertyName("jobRole")]
    public string JobRole { get; set; } = null!;
    
    [JsonPropertyName("systemRole")]
    public string SystemRole { get; set; } = null!;

    [JsonPropertyName("departmentId")]
    public int? DepartmentId { get; set; }

    [JsonPropertyName("departmentName")]
    public string? DepartmentName { get; set; }
}
