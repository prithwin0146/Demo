using System.Text.Json.Serialization;

namespace EmployeeApi.DTOs;

public class EmployeeProjectDto
{
    [JsonPropertyName("employeeProjectId")]
    public int EmployeeProjectId { get; set; }

    [JsonPropertyName("encryptedEmployeeId")]
    public string EncryptedEmployeeId { get; set; } = null!;

    [JsonPropertyName("encryptedProjectId")]
    public string EncryptedProjectId { get; set; } = null!;

    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }

    [JsonPropertyName("employeeName")]
    public string EmployeeName { get; set; } = string.Empty;

    [JsonPropertyName("projectId")]
    public int ProjectId { get; set; }

    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = string.Empty;

    [JsonPropertyName("assignedDate")]
    public DateTime AssignedDate { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
}

public class AssignEmployeeDto
{
    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }

    [JsonPropertyName("projectId")]
    public int ProjectId { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
}
