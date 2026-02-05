using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateProjectDto createDto);
    Task<bool> UpdateAsync(int id, UpdateProjectDto updateDto);
    Task<bool> DeleteAsync(int id);
    Task<PagedResponse<ProjectDto>> GetProjectsPagedAsync(PaginationRequest request);
}
