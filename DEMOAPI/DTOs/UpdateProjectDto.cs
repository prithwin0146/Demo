using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeApi.DTOs;

public class UpdateProjectDto
{
    [Required]
    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = null!;

    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [Required]
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    [Required]
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
}
