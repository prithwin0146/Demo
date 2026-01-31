using System.Text.Json.Serialization;

namespace EmployeeApi.DTOs;

public class CreateEmployeeDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    
    [JsonPropertyName("jobRole")]
    public string JobRole { get; set; } = null!;
    
    [JsonPropertyName("systemRole")]
    public string SystemRole { get; set; } = "Employee";
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}
