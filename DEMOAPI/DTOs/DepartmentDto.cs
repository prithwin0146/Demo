using System.Text.Json.Serialization;

namespace EmployeeApi.DTOs;

public class DepartmentDto
{
    [JsonPropertyName("departmentId")]
    public int DepartmentId { get; set; }

    [JsonPropertyName("encryptedId")]
    public string EncryptedId { get; set; } = null!;

    [JsonPropertyName("departmentName")]
    public string DepartmentName { get; set; } = null!;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("managerId")]
    public int? ManagerId { get; set; }

    [JsonPropertyName("managerName")]
    public string? ManagerName { get; set; }

    [JsonPropertyName("employeeCount")]
    public int EmployeeCount { get; set; }
}
