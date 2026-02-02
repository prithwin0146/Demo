using System.Text.Json.Serialization;

namespace EmployeeApi.DTOs;

public class ProjectDto
{
    [JsonPropertyName("projectId")]
    public int ProjectId { get; set; }

    [JsonPropertyName("projectName")]
    public string ProjectName { get; set; } = null!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;
}
